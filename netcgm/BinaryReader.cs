using netcgm.Buffers;
using netcgm.Commands;
using netcgm.Commands.ControlElements;
using netcgm.Commands.MetafileDescriptorElements;
using netcgm.Extensions;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace netcgm
{
    internal class BinaryReader : IDisposable, IAsyncDisposable
    {
        private readonly Pipe pipe;
        private readonly PipeWriter writer;
        private readonly PipeReader reader;

        private readonly Stream stream;
        private readonly int minimumBufferSize;

        private readonly List<Command> commands;
        private readonly MetafileDescriptor descriptor;

        public BinaryReader(Stream stream, int minimumBufferSize = 2048)
        {
            pipe = new Pipe();
            writer = pipe.Writer;
            reader = pipe.Reader;

            commands = new List<Command>();
            descriptor = new MetafileDescriptor();

            this.stream = stream;
            this.minimumBufferSize = minimumBufferSize;

        }

        private async Task FillPipeAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                Memory<byte> memory = writer.GetMemory(minimumBufferSize);
                int bytesRead = await stream.ReadAsync(memory, cancellationToken);

                if (bytesRead == 0)
                {
                    break;
                }

                writer.Advance(bytesRead);

                FlushResult result = await writer.FlushAsync(cancellationToken);

                if (result.IsCompleted)
                {
                    break;
                }
            }

            writer.Complete();
        }

        private void UpdateMetafileDescriptor(Command command)
        {
            switch (command)
            {
                case VdcType c: descriptor.VdcType = c.Value;  break;
                case IntegerPrecision c: descriptor.IntegerPrecision = c.Precision; break;
                    

                case VdcRealPrecision c: descriptor.VdcRealPrecision = c.Specification; break;
                case VdcIntegerPrecision c: descriptor.VdcIntegerPrecision = c.Precision; break;
                default: break;
            }
        }

        private void AddCommand(Command command)
        {
            commands.Add(command);
            UpdateMetafileDescriptor(command);
        }

        private Command? ProcessLongForm(ref SequenceReader<byte> reader, int elementClass, int elementId)
        {
            BufferSegment<byte> firstPartition = default;
            BufferSegment<byte> lastPartition = default;
            int countOfPartitions = 0;
            bool isLastPartition = false;
            try
            {
                do
                {
                    if (!reader.TryReadBigEndian(out ushort word2))
                        return null;

                    int partitionFlag = (word2 >> 15) & 1;
                    isLastPartition = partitionFlag == 0;

                    int partitionLength = word2 & ~(1 << 15);
                    countOfPartitions++;

                    if (!reader.TryRead(partitionLength, out var partition))
                    {
                        return null;
                    }

                    bool hasNullOctet = partitionLength % 2 == 1;
                    if (hasNullOctet && !reader.TryRead(out var nullOctet))
                    {
                        return null;
                    }

                    if (countOfPartitions == 1 && isLastPartition)
                    {
                        return Command.Create(elementClass, elementId, partition, descriptor);
                    }
                    else
                    {
                        var partitionBuffer = ArrayPool<byte>.Shared.Rent(partitionLength);
                        partition.CopyTo(partitionBuffer.AsMemory(0, partitionLength).Span);

                        lastPartition ??= firstPartition;
                        firstPartition ??= new BufferSegment<byte>(partitionBuffer, 0, partitionLength);
                        lastPartition = lastPartition?.Append(partitionBuffer, 0, partitionLength);
                    }
                } while (!isLastPartition);

                var sequence = new ReadOnlySequence<byte>(firstPartition, 0, lastPartition, lastPartition.Memory.Length);
                return Command.Create(elementClass, elementId, sequence, descriptor);
            }
            finally
            {
                if (firstPartition != null)
                {
                    ArrayPool<byte>.Shared.Return(firstPartition);
                }
            }
        }

        private Command? ProcessShortForm(ref SequenceReader<byte> reader, int elementClass, int elementId, int parameterListLength)
        {
            if (!reader.TryRead(parameterListLength, out var parameterList))
            {
                return null;
            }

            bool hasNullOctet = parameterListLength % 2 == 1;
            if (hasNullOctet && !reader.TryRead(out var nullOctet))
            {
                return null;
            }

            return Command.Create(elementClass, elementId, parameterList, descriptor);
        }

        private void ProcessCommands(ref ReadOnlySequence<byte> buffer)
        {
            var reader = new SequenceReader<byte>(buffer);

            while (reader.TryReadBigEndian(out ushort word))
            {
                var command = default(Command);
                int elementClass = word >> 12; // 4 bits
                int elementId = (word >> 5) & 127; // 7 bits
                int parameterListLength = word & 31; // 5 bits

                if (parameterListLength == 31)
                {
                    command = ProcessLongForm(ref reader, elementClass, elementId);
                }
                else if (reader.Remaining >= parameterListLength)
                {
                    command = ProcessShortForm(ref reader, elementClass, elementId, parameterListLength);
                }

                if (command == null)
                {
                    break;
                }

                AddCommand(command);
                buffer = buffer.Slice(reader.Position);
            }
        }

        private async Task ReadPipeAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                ReadResult result = await reader.ReadAsync(cancellationToken);
                ReadOnlySequence<byte> buffer = result.Buffer;

                ProcessCommands(ref buffer);
                reader.AdvanceTo(buffer.Start, buffer.End);

                if (result.IsCompleted)
                {
                    break;
                }
            }

            reader.Complete();
        }

        public async ValueTask<(MetafileDescriptor, IEnumerable<Command>)> ReadAsync(CancellationToken cancellationToken)
        {

            var writing = FillPipeAsync(cancellationToken);
            var reading = ReadPipeAsync(cancellationToken);

            await Task.WhenAll(writing, reading);

            return (descriptor, commands);
        }

        public void Dispose() => stream.Dispose();
        public ValueTask DisposeAsync() => stream.DisposeAsync();
    }
}
