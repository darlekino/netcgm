using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace netcgm.Commands.GraphicalPrimitiveElements
{
    public class DisjointPolyline : GraphicalPrimitiveElement
    {
        public const int ElementId = 2;

        public IEnumerable<CgmPoint> Points { get; }

        public DisjointPolyline(MetafileDescriptor descriptor, ReadOnlySequence<byte> sequence)
        {
            var reader = new BinaryParameterListReader(descriptor, sequence);
            Points = reader.ReadPointList();
        }
    }
}
