/***************************************************************************
 *   Point.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace UltimaXNA
{
    public interface IPoint2D
    {
        int X { get; }
        int Y { get; }
    }

    public interface IPoint3D : IPoint2D
    {
        int Z { get; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Point2D : IPoint2D
    {
        private int m_x;
        private int m_y;

        public Int32 X
        {
            get { return m_x; }
            set { m_x = value; }
        }
        public Int32 Y
        {
            get { return m_y; }
            set { m_y = value; }
        }

        public Point2D(int x, int y)
        {
            m_x = x;
            m_y = y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point2D))
            {
                return false;
            }

            Point2D pt = (Point2D)obj;

            return pt.X == X && pt.Y == Y;
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }

        public static bool operator ==(Point2D p1, Point2D p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Point2D p1, Point2D p2)
        {
            return !p1.Equals(p2);
        }

        public static Point2D operator +(Point2D p1, Point2D p2)
        {
            return new Point2D(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point2D operator -(Point2D p1, Point2D p2)
        {
            return new Point2D(p1.X - p2.X, p1.Y - p2.Y);
        }

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Point3D : IPoint3D, IComparable, IComparable<Point3D>
    {
        internal int m_X;
        internal int m_Y;
        internal int m_Z;

        public static readonly Point3D Zero = new Point3D(0, 0, 0);

        public Point3D(int x, int y, int z)
        {
            m_X = x;
            m_Y = y;
            m_Z = z;
        }

        public Point3D(IPoint3D p)
            : this(p.X, p.Y, p.Z)
        {
        }

        public Point3D(IPoint2D p, int z)
            : this(p.X, p.Y, z)
        {
        }

        public int X
        {
            get
            {
                return m_X;
            }
            set
            {
                m_X = value;
            }
        }

        public int Y
        {
            get
            {
                return m_Y;
            }
            set
            {
                m_Y = value;
            }
        }

        public int Z
        {
            get
            {
                return m_Z;
            }
            set
            {
                m_Z = value;
            }
        }

        public override string ToString()
        {
            return String.Format("({0}, {1}, {2})", m_X, m_Y, m_Z);
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is IPoint3D))
                return false;

            IPoint3D p = (IPoint3D)o;

            return m_X == p.X && m_Y == p.Y && m_Z == p.Z;
        }

        public override int GetHashCode()
        {
            return m_X ^ m_Y ^ m_Z;
        }

        public static Point3D Parse(string value)
        {
            int start = value.IndexOf('(');
            int end = value.IndexOf(',', start + 1);

            string param1 = value.Substring(start + 1, end - (start + 1)).Trim();

            start = end;
            end = value.IndexOf(',', start + 1);

            string param2 = value.Substring(start + 1, end - (start + 1)).Trim();

            start = end;
            end = value.IndexOf(')', start + 1);

            string param3 = value.Substring(start + 1, end - (start + 1)).Trim();

            return new Point3D(Convert.ToInt32(param1), Convert.ToInt32(param2), Convert.ToInt32(param3));
        }

        public static bool operator ==(Point3D l, Point3D r)
        {
            return l.m_X == r.m_X && l.m_Y == r.m_Y && l.m_Z == r.m_Z;
        }

        public static bool operator !=(Point3D l, Point3D r)
        {
            return l.m_X != r.m_X || l.m_Y != r.m_Y || l.m_Z != r.m_Z;
        }

        public static bool operator ==(Point3D l, IPoint3D r)
        {
            if (Object.ReferenceEquals(r, null))
                return false;

            return l.m_X == r.X && l.m_Y == r.Y && l.m_Z == r.Z;
        }

        public static bool operator !=(Point3D l, IPoint3D r)
        {
            if (Object.ReferenceEquals(r, null))
                return false;

            return l.m_X != r.X || l.m_Y != r.Y || l.m_Z != r.Z;
        }

        public int CompareTo(Point3D other)
        {
            int v = (m_X.CompareTo(other.m_X));

            if (v == 0)
            {
                v = (m_Y.CompareTo(other.m_Y));

                if (v == 0)
                    v = (m_Z.CompareTo(other.m_Z));
            }

            return v;
        }

        public int CompareTo(object other)
        {
            if (other is Point3D)
                return this.CompareTo((Point3D)other);
            else if (other == null)
                return -1;

            throw new ArgumentException();
        }
    }
}
