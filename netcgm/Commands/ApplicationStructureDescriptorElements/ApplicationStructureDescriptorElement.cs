using netcgm.Exceptions;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace netcgm.Commands.ApplicationStructureDescriptorElements
{
    public class ApplicationStructureDescriptorElement: Command
    {
        public const int ElementClass = 9;

        public ApplicationStructureDescriptorElement()
        {
            
        }

        public static ApplicationStructureDescriptorElement Create(int elementId, ReadOnlySequence<byte> parameterList, MetafileDescriptor descriptor)
        {
            return (elementId) switch
            {
                1 => new ApplicationStructureDescriptorElement(),
                _ => throw new UnsupportedElementIdException(elementId)
            };
        }
    }
}
