using netcgm.Buffers;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace netcgm.Extensions
{
    internal static class BufferExtensions
    {
        public static void Return<T>(this ArrayPool<T> pool, BufferSegment<T> segment)
        {
            while (segment != null)
            {
                pool.Return(segment.Array);
                segment = segment.Next as BufferSegment<T>;
            }
        }

        public static bool TryRead<T>(ref this SequenceReader<T> reader, long count, out ReadOnlySequence<T> sequence) where T : unmanaged, IEquatable<T>
        {
            if (reader.Remaining < count)
            {
                sequence = default;
                return false;
            }

            sequence = reader.Sequence.Slice(reader.Consumed, count);
            reader.Advance(count);
            return true;
        }

        public static bool TryReadBigEndian(ref this SequenceReader<byte> reader, out ushort value)
        {
            if (reader.TryRead(out var octet1) && reader.TryRead(out var octet2))
            {
                value = (ushort)((octet1 << 8) | octet2);
                return true;
            }

            value = default;
            return false;
        }
    }
}
