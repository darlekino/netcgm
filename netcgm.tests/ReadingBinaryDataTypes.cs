using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace netcgm.tests
{
    public class ReadingBinaryDataTypes
    {
        static byte[] Float32Bit(float v)
        {
            var bytes = BitConverter.GetBytes(v);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return bytes;
        }

        [Theory]
        [InlineData(5f)]
        [InlineData(5312f)]
        [InlineData(-54.66644f)]
        [InlineData(0f)]
        public void ReadFloat32BitVdcTest(float expected)
        {
            var bytes = Float32Bit(expected);
            var sequence = new ReadOnlySequence<byte>(bytes);
            var descriptor = new MetafileDescriptor
            {
                VdcType = VdcTypeSpecification.Real,
                VdcRealPrecision = RealPrecisionSpecification.FloatingPoint32Bit
            };
            BinaryParameterListReader reader = new BinaryParameterListReader(descriptor, sequence);

            var actual = reader.ReadVdc();

            Assert.Equal(expected, actual);
        }

        static byte[] Float64Bit(double v)
        {
            var bytes = BitConverter.GetBytes(v);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return bytes;
        }

        [Theory]
        [InlineData(5)]
        [InlineData(5312.23)]
        [InlineData(-54.66644)]
        [InlineData(-666)]
        [InlineData(0)]
        public void ReadFloat64BitVdcTest(double expected)
        {
            var bytes = Float64Bit(expected);
            var sequence = new ReadOnlySequence<byte>(bytes);
            var descriptor = new MetafileDescriptor
            {
                VdcType = VdcTypeSpecification.Real,
                VdcRealPrecision = RealPrecisionSpecification.FloatingPoint64Bit
            };
            BinaryParameterListReader reader = new BinaryParameterListReader(descriptor, sequence);

            var actual = reader.ReadVdc();

            Assert.Equal(expected, actual);
        }

        private byte[] FixedPoint32Bit(short whole, ushort fraction)
        {
            var wholeBytes = BitConverter.GetBytes(whole);
            var fractionBytes = BitConverter.GetBytes(fraction);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(wholeBytes);
                Array.Reverse(fractionBytes);
            }

            return wholeBytes.Concat(fractionBytes).ToArray();
        }

        [Theory]
        [InlineData(5, 0)]
        [InlineData(33, 23)]
        [InlineData(-54, 44)]
        [InlineData(-666, 0)]
        [InlineData(0, 0)]
        public void ReadFixed32BitVdcTest(short whole, ushort fraction)
        {
            var expected = whole + fraction / Math.Pow(2, 16);
            var bytes = FixedPoint32Bit(whole, fraction);
            var sequence = new ReadOnlySequence<byte>(bytes);
            var descriptor = new MetafileDescriptor
            {
                VdcType = VdcTypeSpecification.Real,
                VdcRealPrecision = RealPrecisionSpecification.FixedPoint32Bit
            };
            BinaryParameterListReader reader = new BinaryParameterListReader(descriptor, sequence);

            var actual = reader.ReadVdc();

            Assert.Equal(expected, actual);
        }

        private byte[] FixedPoint64Bit(int whole, uint fraction)
        {
            var wholeBytes = BitConverter.GetBytes(whole);
            var fractionBytes = BitConverter.GetBytes(fraction);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(wholeBytes);
                var r = (wholeBytes[0] << 24) +(wholeBytes[1] << 16) + (wholeBytes[2] << 8) + wholeBytes[3];
                Array.Reverse(fractionBytes);
            }

            return wholeBytes.Concat(fractionBytes).ToArray();
        }

        [Theory]
        [InlineData(5, 0)]
        [InlineData(33, 23)]
        [InlineData(-54, 44)]
        [InlineData(-666, 0)]
        [InlineData(0, 0)]
        public void ReadFixed64BitVdcTest(int whole, uint fraction)
        {
            var expected = whole + fraction / Math.Pow(2, 32);
            var bytes = FixedPoint64Bit(whole, fraction);
            var sequence = new ReadOnlySequence<byte>(bytes);
            var descriptor = new MetafileDescriptor
            {
                VdcType = VdcTypeSpecification.Real,
                VdcRealPrecision = RealPrecisionSpecification.FixedPoint64Bit
            };
            BinaryParameterListReader reader = new BinaryParameterListReader(descriptor, sequence);

            var actual = reader.ReadVdc();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReadPointListTest()
        {
            var expected = new List<CgmPoint>
            {
                new CgmPoint(4, 5),
                new CgmPoint(4, 5),
                new CgmPoint(4, 5),
            };

            var bytes = expected.SelectMany(p => Float64Bit(p.X).Concat(Float64Bit(p.Y))).ToArray();
            var sequence = new ReadOnlySequence<byte>(bytes);
            var descriptor = new MetafileDescriptor
            {
                VdcType = VdcTypeSpecification.Real,
                VdcRealPrecision = RealPrecisionSpecification.FloatingPoint64Bit
            };
            BinaryParameterListReader reader = new BinaryParameterListReader(descriptor, sequence);

            var actual = reader.ReadPointList();

            Assert.Equal(expected, actual);
        }
    }
}
