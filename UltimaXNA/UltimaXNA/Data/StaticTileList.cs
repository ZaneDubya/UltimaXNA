#region File Description & Usings
/***************************************************************************
 *                                TileList.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: TileList.cs 4 2006-06-15 04:28:39Z mark $
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
#endregion

namespace UltimaXNA.Data
{
    public class StaticTileList
    {
        private static StaticTile[] m_EmptyTiles = new StaticTile[0];

        private int m_Count;
        private StaticTile[] m_StaticTiles;

        public StaticTileList()
        {
            m_Count = 0;
            m_StaticTiles = new StaticTile[8];
        }

        public int Count
        {
            get { return m_Count; }
        }

        public void AddRange(StaticTile[] tiles)
        {
            if ((m_Count + tiles.Length) > m_StaticTiles.Length)
            {
                StaticTile[] old = m_StaticTiles;

                m_StaticTiles = new StaticTile[(m_Count + tiles.Length) * 2];

                for (int i = 0; i < old.Length; ++i)
                {
                    m_StaticTiles[i] = old[i];
                }
            }

            for (int i = 0; i < tiles.Length; ++i)
            {
                m_StaticTiles[m_Count++] = tiles[i];
            }
        }

        public void Add(short id, sbyte z)
        {
            if ((m_Count + 1) > m_StaticTiles.Length)
            {
                StaticTile[] old = m_StaticTiles;

                m_StaticTiles = new StaticTile[old.Length * 2];

                for (int i = 0; i < old.Length; ++i)
                {
                    m_StaticTiles[i] = old[i];
                }
            }

            m_StaticTiles[m_Count].ID = id;
            m_StaticTiles[m_Count].Z = z;

            m_Count++;
        }

        public StaticTile[] ToArray()
        {
            if (m_Count == 0)
            {
                return m_EmptyTiles;
            }

            StaticTile[] staticTiles = new StaticTile[m_Count];

            for (int i = 0; i < m_Count; ++i)
            {
                staticTiles[i] = m_StaticTiles[i];
            }

            m_Count = 0;

            return staticTiles;
        }
    }
}