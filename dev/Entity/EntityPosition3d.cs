/***************************************************************************
 *   Position3D.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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

namespace UltimaXNA.Entity
{
    public class Position3D : IPoint2D
    {
        public static Vector3 NullPosition = new Vector3(-1);

        Vector3 m_tile;
        Vector3 m_offset;

        public Vector3 Tile_V3 { get { return m_tile; } set { m_tile = value; } }
        public Vector3 Offset_V3 { get { return m_offset; } set { m_offset = value; } }
        public Vector3 Point_V3 { get { return m_tile + m_offset; } }

        public bool IsOffset { get { return (X_offset != 0) || (Y_offset != 0) || (Z_offset != 0); } }
        public bool IsNullPosition { get { return m_tile == NullPosition; } }

        public int X { get { return (int)m_tile.X; } set { m_tile.X = value; } }
        public int Y { get { return (int)m_tile.Y; } set { m_tile.Y = value; } }
        public int Z { get { return (int)m_tile.Z; } set { m_tile.Z = value; } }
        public float X_offset { get { return m_offset.X % 1.0f; } }
        public float Y_offset { get { return m_offset.Y % 1.0f; } }
        public float Z_offset { get { return m_offset.Z; } }

        public Position3D()
        {
            m_tile = NullPosition;
        }

        public Position3D(int x, int y, int z)
        {
            m_tile = new Vector3(x, y, z);
        }

        public Position3D(Vector3 v)
        {
            m_tile = v;
        }

        public override bool Equals(object o)
        {
            if (o == null) return false;
            if (o.GetType() != typeof(Position3D)) return false;
            if (this.X != ((Position3D)o).X) return false;
            if (this.Y != ((Position3D)o).Y) return false;
            if (this.Z != ((Position3D)o).Z) return false;
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
