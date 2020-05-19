using System;
using System.Collections.Generic;
using System.Text;

namespace netcgm
{
    public struct CgmPoint : IEquatable<CgmPoint>
    {
        public CgmPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
        public double X { get; set; }
        public double Y { get; set; }

        public bool Equals(CgmPoint other) => X == other.X && Y == other.Y;
        public static bool operator ==(CgmPoint left, CgmPoint right) => left.Equals(right);
        public static bool operator !=(CgmPoint left, CgmPoint right) => !left.Equals(right);
        public override string ToString() => $"X={X} Y={Y}";
    }
}
