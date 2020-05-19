using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace netcgm.Commands.MetafileDescriptorElements
{
    public class IntegerPrecision : MetafileDescriptorElement
    {
        public const int ElementId = 4;
        public int Precision { get; }

        public IntegerPrecision(MetafileDescriptor descriptor, ReadOnlySequence<byte> sequence)
        {
            var reader = new BinaryParameterListReader(descriptor, sequence);
            Precision = reader.ReadInteger();
        }
    }
}
