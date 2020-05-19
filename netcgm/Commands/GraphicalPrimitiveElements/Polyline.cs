using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace netcgm.Commands.GraphicalPrimitiveElements
{
    public class Polyline : GraphicalPrimitiveElement
    {
        public const int ElementId = 1;

        public IEnumerable<CgmPoint> Points { get; }

        public Polyline(MetafileDescriptor descriptor, ReadOnlySequence<byte> sequence)
        {
            var reader = new BinaryParameterListReader(descriptor, sequence);
            Points = reader.ReadPointList();
        }
    }
}
