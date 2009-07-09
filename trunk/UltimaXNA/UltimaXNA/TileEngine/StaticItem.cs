#region File Description & Usings
//-----------------------------------------------------------------------------
// StaticItem.cs
//
// Created by ClintXNA
//-----------------------------------------------------------------------------
using Microsoft.Xna.Framework;
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
        public int OwnerGUID { get { return -1; } }

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