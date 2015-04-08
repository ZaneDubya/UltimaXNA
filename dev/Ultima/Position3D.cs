/***************************************************************************
 *   Position3D.cs
 *   Based on code from RunUO: http://www.runuo.com
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.UltimaWorld;
#endregion

namespace UltimaXNA
{
    public interface IPoint2D
    {
        int X { get; }
        int Y { get; }
    }

    public class Position3D : IPoint2D
    {
        public static Point NullTile = new Point(int.MinValue, int.MinValue);

        private Point m_Tile;
        private int m_Z;
        private Vector3 m_Offset;

        public Point Tile
        {
            get { return m_Tile; }
            set
            {
                if (m_Tile != value)
                {
                    m_Tile = value;
                    if (m_Tile != NullTile && m_OnTileChanged != null)
                        m_OnTileChanged(m_Tile.X, m_Tile.Y);
                }
            }
        }

        public Vector3 Offset { get { return m_Offset; } set { m_Offset = value; } }

        public bool IsOffset { get { return m_Offset != Vector3.Zero; } }
        public bool IsNullPosition { get { return m_Tile == NullTile; } }

        public int X
        {
            get { return (int)m_Tile.X; }
        }
        public int Y
        {
            get { return (int)m_Tile.Y; }
        }
        public int Z
        {
            get { return (int)m_Z; }
        }

        public float X_offset { get { return m_Offset.X % 1.0f; } }
        public float Y_offset { get { return m_Offset.Y % 1.0f; } }
        public float Z_offset { get { return m_Offset.Z; } }

        private Action<int, int> m_OnTileChanged;

        public Position3D(Action<int, int> onTileChanged)
        {
            Tile = NullTile;
            m_OnTileChanged = onTileChanged;
        }

        public Position3D(int x, int y, int z)
        {
            Tile = new Point(x, y);
            m_Z = z;
        }

        internal void Set(int x, int y, int z)
        {
            m_Z = z;
            Tile = new Point(x, y);
            m_Offset = Vector3.Zero;
        }

        public override bool Equals(object o)
        {
            if (o == null) return false;
            if (o.GetType() != typeof(Position3D)) return false;
            if (X != ((Position3D)o).X) return false;
            if (Y != ((Position3D)o).Y) return false;
            if (Z != ((Position3D)o).Z) return false;
            return true;
        }

        // Equality operator. Returns dbNull if either operand is dbNull, 
        // otherwise returns dbTrue or dbFalse:
        public static bool operator ==(Position3D x, Position3D y)
        {
            if ((object)x == null)
                return ((object)y == null);
            return x.Equals(y);
        }

        // Inequality operator. Returns dbNull if either operand is
        // dbNull, otherwise returns dbTrue or dbFalse:
        public static bool operator !=(Position3D x, Position3D y)
        {
            if ((object)x == null)
                return ((object)y != null);
            return !x.Equals(y);
        }

        public override int GetHashCode()
        {
            return X ^ Y ^ Z;
        }

        public override string ToString()
        {
            return string.Format("X:{0} Y:{1} Z:{2}", X, Y, Z);
        }

        public string ToStringComplex()
        {
            return
                "P(Tile)=" + ToString() + Environment.NewLine +
                "P(Ofst)=" + string.Format("X:{0:0.00} Y:{1:0.00} Z:{2:0.00}", X_offset, Y_offset, Z_offset) + Environment.NewLine +
                "D(Tile)=" + string.Format("X:{0:0.00} Y:{1:0.00} Z:{2:0.00}", X, Y, Z) + Environment.NewLine +
                "D(Ofst)=" + string.Format("X:{0:0.00} Y:{1:0.00} Z:{2:0.00}", X_offset, Y_offset, Z_offset);
        }
    }
}
