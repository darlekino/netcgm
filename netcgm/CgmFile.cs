using netcgm.Commands;
using netcgm.Commands.AttributeElements;
using netcgm.Commands.MetafileDescriptorElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace netcgm
{
    public class CgmFile
    {
        public MetafileDescriptor Descriptor { get; }
        public IEnumerable<Command> Commands { get; }

        private CgmFile(MetafileDescriptor descriptor, IEnumerable<Command> commands)
        {
            Descriptor = descriptor;
            Commands = commands;
        }

        public static async ValueTask<CgmFile> ReadBinaryFormatAsync(string fileName, CancellationToken cancellationToken = default)
        {
            await using (var fileStrem = File.OpenRead(fileName))
            {
                return await CgmFile.ReadBinaryFormatAsync(fileStrem);
            }

        }

        public static async ValueTask<CgmFile> ReadBinaryFormatAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            var binaryReader = new BinaryReader(stream);
            var (descriptor, commands) = await binaryReader.ReadAsync(cancellationToken);
            return new CgmFile(descriptor, commands);
        }
    }
}
