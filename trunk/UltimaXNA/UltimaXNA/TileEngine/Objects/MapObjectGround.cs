/***************************************************************************
 *   MapObjectGround.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entities;
#endregion

namespace UltimaXNA.TileEngine
{
    public class MapObjectGround : MapObject
    {
        private Surroundings _surroundingTiles;
        public Vector3[] Normals;

        public MapObjectGround(Data.Tile landTile, Vector3 position)
            : base(position)
        {
            ItemID = landTile.ID;
            Normals = new Vector3[4];
        }

        public bool Ignored
        {
            get { return (ItemID == 2 || ItemID == 0x1DB || (ItemID >= 0x1AE && ItemID <= 0x1B5)); }
        }

        public Surroundings Surroundings
        {
            get { return _surroundingTiles; }
            set { _surroundingTiles = value; }
        }

        public void CalculateNormals(
            int NorthWest0, int NorthWest2, int NorthEast0, int NorthEast1,
            int SouthWest2, int SouthWest3, int SouthEast1, int SouthEast3)
        {
            Normals[0] = m_CalculateNormal(
                NorthWest0, this.Surroundings.East,
                NorthEast0, this.Surroundings.South);
            Normals[1] = m_CalculateNormal(
                this.Z, SouthEast1,
                NorthEast1, this.Surroundings.Down);
            Normals[2] = m_CalculateNormal(
                NorthWest2, this.Surroundings.Down,
                this.Z, SouthWest2);
            Normals[3] = m_CalculateNormal(
                this.Surroundings.South, SouthEast3,
                this.Surroundings.East, SouthWest3);
        }

        private Vector3 m_CalculateNormal(float A, float B, float C, float D)
        {
            Vector3 iVector = new Vector3(
                (A - B) / 2f,
                1f,
                (C - D) / 2f);
            iVector.Normalize();
            return iVector;
        }
    }

    public class Surroundings
    {
        public int Down;
        public int East;
        public int South;

        public Surroundings(int nDown, int nEast, int nSouth)
        {
            Down = nDown;
            East = nEast;
            South = nSouth;
        }
    }
}