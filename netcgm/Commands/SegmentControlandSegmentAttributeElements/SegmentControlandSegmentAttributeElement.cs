using netcgm.Exceptions;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace netcgm.Commands.SegmentControlandSegmentAttributeElements
{
    public class SegmentControlandSegmentAttributeElement : Command
    {
        public const int ElementClass = 8;

        public SegmentControlandSegmentAttributeElement()
        {
            
        }

        public static SegmentControlandSegmentAttributeElement Create(int elementId, ReadOnlySequence<byte> parameterList, MetafileDescriptor descriptor)
        {
            return (elementId) switch
            {
                _ when (elementId >= 1 && elementId <= 7) => new SegmentControlandSegmentAttributeElement(),
                _ => throw new UnsupportedElementIdException(elementId)
            };
        }
    }
}
