using netcgm.Exceptions;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace netcgm.Commands.ControlElements
{
    public class ControlElement: Command
    {
        public const int ElementClass = 3;

        public ControlElement()
        {
            
        }

        public static ControlElement Create(int elementId, ReadOnlySequence<byte> parameterList, MetafileDescriptor descriptor)
        {
            return (elementId) switch
            {
                _ when (elementId >= 0 && elementId <= 20) => new ControlElement(),
                _ => throw new UnsupportedElementIdException(elementId)
            };
        }
    }
}
