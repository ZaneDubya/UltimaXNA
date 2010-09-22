/***************************************************************************
 *   Position3D.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from RunUO: http://www.runuo.com
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
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
using UltimaXNA.TileEngine;
#endregion

namespace UltimaXNA.Entities
{
    public class Position3D : IPoint2D
    {
        public static Vector3 NullPosition = new Vector3(-1);

        Vector3 _tile;
        Vector3 _offset;

        public Vector3 Tile_V3 { get { return _tile; } set { _tile = value; } }
        public Vector3 Offset_V3 { get { return _offset; } set { _offset = value; } }
        public Vector3 Point_V3 { get { return _tile + _offset; } }

        public bool IsOffset { get { return (X_offset != 0) || (Y_offset != 0) || (Z_offset != 0); } }
        public bool IsNullPosition { get { return _tile == NullPosition; } }

        public int X { get { return (int)_tile.X; } set { _tile.X = value; } }
        public int Y { get { return (int)_tile.Y; } set { _tile.Y = value; } }
        public int Z { get { return (int)_tile.Z; } set { _tile.Z = value; } }
        float X_offset { get { return _offset.X % 1.0f; } }
        float Y_offset { get { return _offset.Y % 1.0f; } }
        float Z_offset { get { return _offset.Z % 1.0f; } }

        public int Draw_TileX { get { return drawOffsetTile(X, X_offset); } }
        public int Draw_TileY { get { return drawOffsetTile(Y, Y_offset); } }
        public float Draw_Xoffset { get { return drawOffsetOffset(X_offset); } }
        public float Draw_Yoffset { get { return drawOffsetOffset(Y_offset); } }
        public float Draw_Zoffset { get { return Z_offset; } }

        int drawOffsetTile(int tile, float offset)
        {
            return (offset > 0) ? tile + 1 : tile;
        }

        float drawOffsetOffset(float offset)
        {
            if (offset > 0)
                return offset - 1f;
            else
                return offset;
            // return (offset == 0) ? 0 : offset - 1f; 
        }

        public Position3D()
        {
            _tile = NullPosition;
        }

        public Position3D(int x, int y, int z)
        {
            _tile = new Vector3(x, y, z);
        }

        public Position3D(Vector3 v)
        {
            _tile = v;
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
                "PT=" + ToString() + Environment.NewLine +
                "PO=" + string.Format("X:{0} Y:{1} Z:{2}", X_offset, Y_offset, Z_offset) + Environment.NewLine +
                "DT=" + string.Format("X:{0} Y:{1} Z:{2}", Draw_TileX, Draw_TileY, Z) + Environment.NewLine +
                "D=" + string.Format("X:{0} Y:{1} Z:{2}", Draw_Xoffset, Draw_Yoffset, Draw_Zoffset);
        }
    }
}
