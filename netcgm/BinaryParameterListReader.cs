using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;

namespace netcgm
{
    internal class BinaryParameterListReader
    {
        private readonly MetafileDescriptor descriptor;
        private readonly ReadOnlySequence<byte> sequence;

        public long Position { get; private set; } = 0;
        public long Remaining => sequence.Length - Position;

        private const int sizeOfFixedPoint32 = 2 + 2;
        private const int sizeOfFixedPoint64 = 4 + 4;
        private const int sizeOfFloatingPoint32 = 2 * 2;
        private const int sizeOfFloatingPoint64 = 2 * 4;

        public int SizeOfVdc
        {
            get
            {
                if (descriptor.VdcType == VdcTypeSpecification.Integer)
                {
                    var precision = descriptor.VdcIntegerPrecision;
                    return (precision / 8);
                }

                if (descriptor.VdcType == VdcTypeSpecification.Real)
                {
                    var precision = descriptor.VdcRealPrecision;

                    switch (precision)
                    {
                        case RealPrecisionSpecification.FloatingPoint32Bit: return sizeOfFloatingPoint32;
                        case RealPrecisionSpecification.FloatingPoint64Bit: return sizeOfFloatingPoint64;
                        case RealPrecisionSpecification.FixedPoint32Bit: return sizeOfFixedPoint32;
                        case RealPrecisionSpecification.FixedPoint64Bit: return sizeOfFixedPoint64;
                    }

                    throw new NotSupportedException($"The current Real Precision ({precision}) is not supported");
                }

                throw new NotSupportedException($"The current vdc type ({descriptor.VdcType}) is not supported");
            }
        }

        public int SizeOfPoint => 2 * SizeOfVdc;

        public BinaryParameterListReader(MetafileDescriptor descriptor, ReadOnlySequence<byte> sequence)
        {
            this.descriptor = descriptor;
            this.sequence = sequence;
        }

        public bool HasMoreData() => HasMoreData(1);
        public bool HasMoreData(int minimumLeft) => minimumLeft <= Remaining;

        private byte ReadByte()
        {
            var reader = new SequenceReader<byte>(sequence);
            reader.Advance(Position);

            if (reader.TryRead(out var b))
            {
                Position = reader.Consumed;
                return b;
            }

            throw new InvalidOperationException();
        }

        private void ReadBytes(Span<byte> span)
        {
            if (sequence.Length - Position < span.Length)
            {
                throw new InvalidOperationException();
            }

            sequence.Slice(Position, span.Length).CopyTo(span);
            Position += span.Length;
        }

        private double ReadReal(RealPrecisionSpecification precision)
        {
            switch (precision)
            {
                case RealPrecisionSpecification.FloatingPoint32Bit: return ReadFloatingPoint(4);
                case RealPrecisionSpecification.FloatingPoint64Bit: return ReadFloatingPoint(8);
                case RealPrecisionSpecification.FixedPoint32Bit: return ReadFixedPoint(4);
                case RealPrecisionSpecification.FixedPoint64Bit: return ReadFixedPoint(8);
            }

            throw new NotSupportedException($"The current Real Precision ({precision}) is not supported");
        }

        private int ReadInt(int numBytes)
        {
            if (numBytes < 1 || numBytes > 4)
            {
                throw new ArgumentOutOfRangeException("numBytes", numBytes, "Number of bytes must be between 1 and 4");
            }

            Span<byte> span = stackalloc byte[numBytes];
            ReadBytes(span);

            if (numBytes == 3)
            {
                return ((sbyte)span[0]) << 16 | span[1] << 8 | span[2];
            }

            if (BitConverter.IsLittleEndian)
            {
                span.Reverse();
            }

            switch (numBytes)
            {
                case 1: return BitConverter.ToChar(span);
                case 2: return BitConverter.ToInt16(span);
                case 4: return BitConverter.ToInt32(span);
            }

            throw new NotSupportedException($"The current integer (int with {numBytes} bytes) is not supported");
        }

        private uint ReadUInt(int numBytes)
        {
            if (numBytes < 1 || numBytes > 4)
            {
                throw new ArgumentOutOfRangeException("numBytes", numBytes, "Number of bytes must be between 1 and 4");
            }

            Span<byte> span = stackalloc byte[numBytes];
            ReadBytes(span);

            if (numBytes == 3)
            {
                return (uint)(span[0] << 16 | span[1] << 8 | span[2]);
            }

            if (BitConverter.IsLittleEndian)
            {
                span.Reverse();
            }

            switch (numBytes)
            {
                case 1: return span[0];
                case 2: return BitConverter.ToUInt16(span);
                case 4: return BitConverter.ToUInt32(span);
            }

            throw new NotSupportedException($"The current integer (int with {numBytes} bytes) is not supported");
        }

        private double ReadFixedPoint(int numBytes)
        {
            // ISO/IEC 8632-3 6.4
            // real value is computed as "whole + (fraction / 2**exp)"
            // exp is the width of the fraction value
            // the "whole part" has the same form as a Signed Integer
            int whole = ReadInt(numBytes / 2);
            // the "fractional part" has the same form as an Unsigned Integer
            uint fraction = ReadUInt(numBytes / 2);
            // if someone wanted a 4 byte fixed point real, they get 32 bits (16 bits whole, 16 bits fraction)
            // therefore exp would be 16 here (same for 8 byte with 64 bits and 32/32 -> 32 exp)
            int exp = numBytes / 2 * 8;
            return whole + fraction / Math.Pow(2, exp);
        }

        private double ReadFloatingPoint(int numBytes)
        {
            // ISO/IEC 8632-3 6.5
            // C# float/double conform to ANSI/IEEE 754 and have the same format as the specification wants;
            // but the endianness might not work out. swap if necessary
            if (numBytes == 4)
            {
                Span<byte> floatBytes = stackalloc byte[4];
                ReadBytes(floatBytes);

                if (BitConverter.IsLittleEndian)
                {
                    floatBytes.Reverse();
                }

                return BitConverter.ToSingle(floatBytes);
            }
            if (numBytes == 8)
            {
                Span<byte> doubleBytes = stackalloc byte[8];
                ReadBytes(doubleBytes);

                if (BitConverter.IsLittleEndian)
                {
                    doubleBytes.Reverse();
                }

                return BitConverter.ToDouble(doubleBytes);
            }

            throw new InvalidOperationException(string.Format("Sorry, cannot read a floating point value with {0} bytes", numBytes));
        }

        public int ReadInteger()
        {
            return ReadInt(descriptor.IntegerPrecision / 8);
        }

        public double ReadVdc()
        {
            switch (descriptor.VdcType)
            {
                case VdcTypeSpecification.Real: return ReadReal(descriptor.VdcRealPrecision);
                case VdcTypeSpecification.Integer: return ReadInt(descriptor.VdcIntegerPrecision / 8);
            }

            throw new NotSupportedException($"The current vdc type ({descriptor.VdcType}) is not supported");
        }

        public CgmPoint ReadPoint() => new CgmPoint(ReadVdc(), ReadVdc());

        public IEnumerable<CgmPoint> ReadPointList()
        {
            var points = new List<CgmPoint>();

            while (HasMoreData(SizeOfPoint))
            {
                points.Add(ReadPoint());
            }

            return points;
        }

        public T ReadEnum<T>() where T : Enum
        {
            var value = ReadInt(2);
            return (T)Enum.ToObject(typeof(T), value);
        }
    }
}
