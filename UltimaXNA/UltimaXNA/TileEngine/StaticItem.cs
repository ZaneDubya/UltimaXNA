/***************************************************************************
 *   StaticItem.cs
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
using UltimaXNA.Entities;
#endregion

namespace UltimaXNA.TileEngine
{
    public struct StaticItem : IMapObject
    {
        private int m_ID;
        private int m_SortInfluence;
        private int m_Z;
        private Vector2 m_Position;
        public Vector2 Position { get { return m_Position; } }
        public int OwnerSerial { get { return -1; } }
        public Entity OwnerEntity { get { return null; } }

        public StaticItem(int id, int z, int sortInfluence, Vector2 nPosition)
        {
            m_ID = id;
            m_SortInfluence = sortInfluence;
            m_Z = z;
            m_Position = nPosition;
        }

        public StaticItem(Data.StaticTile staticTile, int sortInfluence, Vector2 nPosition)
        {
            m_ID = staticTile.ID;
            m_SortInfluence = sortInfluence;
            m_Z = staticTile.Z;
            m_Position = nPosition;
        }

        public int ID
        {
            get { return m_ID; }
        }

        public bool Ignored
        {
            get { return (m_ID <= 1); } //  || TileData.ItemData[m_ID - 0x4000].Roof
        }

        public int SortZ
        {
            get { return m_Z; }
        }

        public int Threshold
        {
            get
            {
                Data.ItemData itemData = Data.TileData.ItemData[m_ID & 0x3FFF];

                int background;

                if (itemData.Background)
                {
                    background = 0;
                }
                else
                {
                    background = 1;
                }

                return itemData.Height == 0 ? background : background + 1;
            }
        }

        public int Tiebreaker
        {
            get { return m_SortInfluence; }
        }

        public MapObjectTypes Type
        {
            get { return MapObjectTypes.StaticTile; }
        }

        public int Z
        {
            get { return m_Z; }
        }
    }
}