﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace netcgm.Commands.ControlElements
{
    class VdcRealPrecision : ControlElement
    {
        public const int ElementId = 2;

        public RealRepresentation RepresentationForm { get; }
        public int ExponentWidth { get; }
        public int FractionWidth { get; }
        public RealPrecisionSpecification Specification { get; }

        public VdcRealPrecision(MetafileDescriptor descriptor, ReadOnlySequence<byte> sequence)
        {
            var reader = new BinaryParameterListReader(descriptor, sequence);
            var representationForm = reader.ReadEnum<RealRepresentation>();
            var exponentWidth = reader.ReadInteger();
            var fractionWidth = reader.ReadInteger();

            RepresentationForm = representationForm;
            ExponentWidth = exponentWidth;
            FractionWidth = fractionWidth;

            if (RepresentationForm == RealRepresentation.FloatingPoint)
            {
                if (ExponentWidth == 9 && FractionWidth == 23)
                    Specification = RealPrecisionSpecification.FloatingPoint32Bit;
                else if (ExponentWidth == 12 && FractionWidth == 52)
                    Specification = RealPrecisionSpecification.FloatingPoint64Bit;
            }
            else if (RepresentationForm == RealRepresentation.FixedPoint)
            {
                if (ExponentWidth == 16 && FractionWidth == 16)
                    Specification = RealPrecisionSpecification.FixedPoint32Bit;
                else if (ExponentWidth == 32 && FractionWidth == 32)
                    Specification = RealPrecisionSpecification.FixedPoint64Bit;
            }
        }
    }
}
