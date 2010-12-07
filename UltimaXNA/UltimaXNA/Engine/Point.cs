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
        private int _x;
        private int _y;

        public Int32 X
        {
            get { return _x; }
            set { _x = value; }
        }
        public Int32 Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public Point2D(int x, int y)
        {
            _x = x;
            _y = y;
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
        internal int _X;
        internal int _Y;
        internal int _Z;

        public static readonly Point3D Zero = new Point3D(0, 0, 0);

        public Point3D(int x, int y, int z)
        {
            _X = x;
            _Y = y;
            _Z = z;
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
                return _X;
            }
            set
            {
                _X = value;
            }
        }

        public int Y
        {
            get
            {
                return _Y;
            }
            set
            {
                _Y = value;
            }
        }

        public int Z
        {
            get
            {
                return _Z;
            }
            set
            {
                _Z = value;
            }
        }

        public override string ToString()
        {
            return String.Format("({0}, {1}, {2})", _X, _Y, _Z);
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is IPoint3D))
                return false;

            IPoint3D p = (IPoint3D)o;

            return _X == p.X && _Y == p.Y && _Z == p.Z;
        }

        public override int GetHashCode()
        {
            return _X ^ _Y ^ _Z;
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
            return l._X == r._X && l._Y == r._Y && l._Z == r._Z;
        }

        public static bool operator !=(Point3D l, Point3D r)
        {
            return l._X != r._X || l._Y != r._Y || l._Z != r._Z;
        }

        public static bool operator ==(Point3D l, IPoint3D r)
        {
            if (Object.ReferenceEquals(r, null))
                return false;

            return l._X == r.X && l._Y == r.Y && l._Z == r.Z;
        }

        public static bool operator !=(Point3D l, IPoint3D r)
        {
            if (Object.ReferenceEquals(r, null))
                return false;

            return l._X != r.X || l._Y != r.Y || l._Z != r.Z;
        }

        public int CompareTo(Point3D other)
        {
            int v = (_X.CompareTo(other._X));

            if (v == 0)
            {
                v = (_Y.CompareTo(other._Y));

                if (v == 0)
                    v = (_Z.CompareTo(other._Z));
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
