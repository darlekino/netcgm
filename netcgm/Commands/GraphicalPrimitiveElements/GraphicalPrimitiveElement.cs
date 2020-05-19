using netcgm.Exceptions;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace netcgm.Commands.GraphicalPrimitiveElements
{
    public class GraphicalPrimitiveElement : Command
    {
        public const int ElementClass = 4;

        public GraphicalPrimitiveElement()
        {
            
        }

        public static GraphicalPrimitiveElement Create(int elementId, ReadOnlySequence<byte> parameterList, MetafileDescriptor descriptor)
        {
            return (elementId) switch
            {
                Polyline.ElementId => new Polyline(descriptor, parameterList),
                DisjointPolyline.ElementId => new DisjointPolyline(descriptor, parameterList),
                _ when (elementId >= 3 && elementId <= 29) => new GraphicalPrimitiveElement(),
                _ => throw new UnsupportedElementIdException(elementId)
            };
        }
    }
}
