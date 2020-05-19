using netcgm.Exceptions;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace netcgm.Commands.AttributeElements
{
    public class AttributeElement : Command
    {
        public const int ElementClass = 5;

        public AttributeElement()
        {
            
        }

        public static AttributeElement Create(int elementId, ReadOnlySequence<byte> parameterList, MetafileDescriptor descriptor)
        {
            return (elementId) switch
            {
                _ when (elementId >= 1 && elementId <= 51) => new AttributeElement(),
                _ => throw new UnsupportedElementIdException(elementId)
            };
        }
    }
}
