using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace UltimaXNA.Input
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Point2D
    {
        public Int32 x;
        public Int32 y;

        public override bool Equals(object obj)
        {
            if (!(obj is Point2D))
            {
                return false;
            }

            Point2D pt = (Point2D)obj;

            return pt.x == x && pt.y == y;
        }

        public override int GetHashCode()
        {
            return x ^ y;
        }

        public static bool operator ==(Point2D p1, Point2D p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Point2D p1, Point2D p2)
        {
            return !p1.Equals(p2);
        }
    }
}
