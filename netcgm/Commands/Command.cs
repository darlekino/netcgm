using netcgm.Commands.ApplicationStructureDescriptorElements;
using netcgm.Commands.AttributeElements;
using netcgm.Commands.ControlElements;
using netcgm.Commands.DelimeterElements;
using netcgm.Commands.EscapeElements;
using netcgm.Commands.ExternalElements;
using netcgm.Commands.GraphicalPrimitiveElements;
using netcgm.Commands.MetafileDescriptorElements;
using netcgm.Commands.PictureDescriptorElements;
using netcgm.Commands.SegmentControlandSegmentAttributeElements;
using netcgm.Exceptions;
using System;
using System.Buffers;

namespace netcgm.Commands
{
    public abstract class Command
    {
        public static Command Create(int elementClass, int elementId, ReadOnlySequence<byte> parameterList, MetafileDescriptor description)
        {
            return (elementClass) switch
            {
                DelimiterElement.ElementClass => DelimiterElement.Create(elementId, parameterList, description),
                MetafileDescriptorElement.ElementClass => MetafileDescriptorElement.Create(elementId, parameterList, description),
                PictureDescriptorElement.ElementClass => PictureDescriptorElement.Create(elementId, parameterList, description),
                ControlElement.ElementClass => ControlElement.Create(elementId, parameterList, description),
                GraphicalPrimitiveElement.ElementClass => GraphicalPrimitiveElement.Create(elementId, parameterList, description),
                AttributeElement.ElementClass => AttributeElement.Create(elementId, parameterList, description),
                EscapeElement.ElementClass => new EscapeElement(),
                ExternalElement.ElementClass => ExternalElement.Create(elementId, parameterList, description),
                SegmentControlandSegmentAttributeElement.ElementClass => SegmentControlandSegmentAttributeElement.Create(elementId, parameterList, description),
                ApplicationStructureDescriptorElement.ElementClass => ApplicationStructureDescriptorElement.Create(elementId, parameterList, description),
                _ => throw new UnsupportedElementClassException(elementClass)
            };
        }
    }
}
