using System;
using System.Collections.Generic;
using System.Text;

namespace netcgm
{
    public class MetafileDescriptor
    {
        public int IntegerPrecision { get; internal set; }
        public VdcTypeSpecification VdcType { get; internal set; }
        public int VdcIntegerPrecision { get; internal set; }
        public RealPrecisionSpecification VdcRealPrecision { get; internal set; }

        public MetafileDescriptor()
        {
            IntegerPrecision = 16;
            VdcType = VdcTypeSpecification.Integer;
            VdcIntegerPrecision = 16;
            VdcRealPrecision = RealPrecisionSpecification.FixedPoint32Bit;
        }
    }
}
