/***************************************************************************
 *   Shared.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
#endregion

namespace UltimaXNA
{
    [Flags]
    public enum TileFlag
    {
        None = 0x00000000,
        Background = 0x00000001,
        Weapon = 0x00000002,
        Transparent = 0x00000004,
        Translucent = 0x00000008,
        Wall = 0x00000010,
        Damaging = 0x00000020,
        Impassable = 0x00000040,
        Wet = 0x00000080,
        Unknown1 = 0x00000100,
        Surface = 0x00000200,
        Bridge = 0x00000400,
        Generic = 0x00000800,
        Window = 0x00001000,
        NoShoot = 0x00002000,
        ArticleA = 0x00004000,
        ArticleAn = 0x00008000,
        Internal = 0x00010000,
        Foliage = 0x00020000,
        PartialHue = 0x00040000,
        Unknown2 = 0x00080000,
        Map = 0x00100000,
        Container = 0x00200000,
        Wearable = 0x00400000,
        LightSource = 0x00800000,
        Animation = 0x01000000,
        NoDiagonal = 0x02000000,
        Unknown3 = 0x04000000,
        Armor = 0x08000000,
        Roof = 0x10000000,
        Door = 0x20000000,
        StairBack = 0x40000000,
        StairRight = unchecked((int)0x80000000)
    }

    public enum Direction : byte
    {
        North = 0x0,
        Right = 0x1,
        East = 0x2,
        Down = 0x3,
        South = 0x4,
        Left = 0x5,
        West = 0x6,
        Up = 0x7,
        FacingMask = 0x7,
        Running = 0x80,
        ValueMask = 0x87,
        Nothing = 0xED
    }

    public enum MessageType
    {
        Regular = 0x00,
        System = 0x01,
        Emote = 0x02,
        Label = 0x06,
        Focus = 0x07,
        Whisper = 0x08,
        Yell = 0x09,
        Spell = 0x0A,

        UIld = 0x0D,
        Alliance = 0x0E,
        Command = 0x0F,

        Encoded = 0xC0
    }

    public interface IPoint2D
    {
        int X { get; }
        int Y { get; }
    }

    public interface IPoint3D : IPoint2D
    {
        int Z { get; }
    }

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


    /// <summary>
    /// Status level used to filter out events from IStatusNotifier
    /// </summary>
    public class StatusLevel
    {
        /// <summary>
        /// Indicates Debug level of status
        /// </summary>
        public const int Debug = 0;
        /// <summary>
        /// Indicates Informational level of status
        /// </summary>
        public const int Info = 1;
        /// <summary>
        /// Indicates Warning level of status
        /// </summary>
        public const int Warn = 2;
        /// <summary>
        /// Indicates Error level of status
        /// </summary>
        public const int Error = 3;
        /// <summary>
        /// Indicates Fatal level of status
        /// </summary>
        public const int Fatal = 4;
    }
}