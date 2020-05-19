using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace netcgm.Commands.ControlElements
{
    public class VdcIntegerPrecision: ControlElement
    {
        public const int ElementId = 1;

        public int Precision { get; private set; }

        public VdcIntegerPrecision(MetafileDescriptor descriptor, ReadOnlySequence<byte> sequence)
        {
            var reader = new BinaryParameterListReader(descriptor, sequence);
            Precision = reader.ReadInteger();
        }
    }
}
