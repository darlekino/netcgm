using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace netcgm.Commands.MetafileDescriptorElements
{
    public class VdcType : MetafileDescriptorElement
    {
        public const int ElementId = 3;

        public VdcTypeSpecification Value { get; }

        public VdcType(MetafileDescriptor descriptor, ReadOnlySequence<byte> sequence)
        {
            var reader = new BinaryParameterListReader(descriptor, sequence);
            Value = reader.ReadEnum<VdcTypeSpecification>();
        }
    }
}
