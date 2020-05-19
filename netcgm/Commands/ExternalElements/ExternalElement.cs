using netcgm.Exceptions;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace netcgm.Commands.ExternalElements
{
    public class ExternalElement: Command
    {
        public const int ElementClass = 7;

        public ExternalElement()
        {
            
        }

        public static ExternalElement Create(int elementId, ReadOnlySequence<byte> parameterList, MetafileDescriptor descriptor)
        {
            return (elementId) switch
            {
                _ when (elementId >= 1 && elementId <= 2) => new ExternalElement(),
                _ => throw new UnsupportedElementIdException(elementId)
            };
        }
    }
}
