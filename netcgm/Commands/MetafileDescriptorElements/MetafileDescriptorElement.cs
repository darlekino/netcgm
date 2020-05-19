using netcgm.Exceptions;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace netcgm.Commands.MetafileDescriptorElements
{
    public class MetafileDescriptorElement : Command
    {
        public const int ElementClass = 1;

        public static MetafileDescriptorElement Create(int elementId, ReadOnlySequence<byte> parameterList, MetafileDescriptor descriptor)
        {
            return (elementId) switch
            {
                VdcType.ElementId => new VdcType(descriptor, parameterList),
                IntegerPrecision.ElementId => new IntegerPrecision(descriptor, parameterList),
                _ when (elementId >= 1 && elementId <= 24) => new MetafileDescriptorElement(),
                _ => throw new UnsupportedElementIdException(elementId)
            };
        }
    }
}
