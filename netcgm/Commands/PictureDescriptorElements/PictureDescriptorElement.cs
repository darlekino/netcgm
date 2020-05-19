using netcgm.Exceptions;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace netcgm.Commands.PictureDescriptorElements
{
    public class PictureDescriptorElement : Command
    {
        public const int ElementClass = 2;

        public PictureDescriptorElement()
        {
            
        }

        public static PictureDescriptorElement Create(int elementId, ReadOnlySequence<byte> parameterList, MetafileDescriptor descriptor)
        {
            return (elementId) switch
            {
                _ when (elementId >= 1 && elementId <= 20) => new PictureDescriptorElement(),
                _ => throw new UnsupportedElementIdException(elementId)
            };
        }
    }
}
