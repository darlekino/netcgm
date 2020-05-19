using netcgm.Exceptions;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace netcgm.Commands.DelimeterElements
{
    public class DelimiterElement: Command
    {
        public const int ElementClass = 0;

        public static DelimiterElement Create(int elementId, ReadOnlySequence<byte> parameterList, MetafileDescriptor descriptor)
        {
            return (elementId) switch
            {
                _ when (elementId >= 0 && elementId <= 9) => new DelimiterElement(),
                _ when (elementId >= 13 && elementId <= 23) => new DelimiterElement(),
                _ => throw new UnsupportedElementIdException(elementId)
            };
        }
    }
}
