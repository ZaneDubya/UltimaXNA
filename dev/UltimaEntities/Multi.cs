/***************************************************************************
 *   Multi.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using UltimaXNA.UltimaData;
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaWorld.View;
using Microsoft.Xna.Framework;
using UltimaXNA.UltimaWorld.Model;
#endregion

namespace UltimaXNA.UltimaEntities
{
    class Multi : AEntity
    {
        private static List<Multi> s_RegisteredMultis = new List<Multi>();

        private static void RegisterForMapBlockLoads(Multi multi)
        {
            if (!s_RegisteredMultis.Contains(multi))
                s_RegisteredMultis.Add(multi);
        }

        private static void UnregisterForMapBlockLoads(Multi multi)
        {
            if (s_RegisteredMultis.Contains(multi))
                s_RegisteredMultis.Remove(multi);
        }

        public static void AnnounceMapBlockLoaded(MapBlock block)
        {
            for (int i = 0; i < s_RegisteredMultis.Count; i++)
                if (!s_RegisteredMultis[i].IsDisposed)
                    s_RegisteredMultis[i].ReceiveMapBlockLoaded(block);
        }

        private void ReceiveMapBlockLoaded(MapBlock block)
        {

        }

        MultiComponentList m_Components;

        int m_customHouseRevision = 0x7FFFFFFF;
        StaticTile[] m_customHouseTiles;
        public int CustomHouseRevision { get { return m_customHouseRevision; } }

        bool m_hasCustomTiles = false;
        CustomHouse m_customHouse;
        public void AddCustomHousingTiles(CustomHouse house)
        {
            m_hasCustomTiles = true;
            m_customHouse = house;
            m_customHouseTiles = house.GetStatics(m_Components.Width, m_Components.Height);
        }

        int m_StaticID;
        public int StaticID
        {
            get { return m_StaticID; }
            set
            {
                if (m_StaticID != value)
                {
                    m_StaticID = value;
                }
            }
        }

        void redrawAllTiles()
        {
            m_Components = MultiData.GetComponents(m_StaticID);
            m_unloadedTiles.Clear();
            for (int y = 0; y < m_Components.Height + 1; y++)
            {
                for (int x = 0; x < m_Components.Width; x++)
                {
                    Point p = new Point();
                    p.X = x;
                    p.Y = y;
                    m_unloadedTiles.Add(p);
                }
            }
        }

        public Multi(Serial serial, Map map)
			: base(serial, map)
		{
            RegisterForMapBlockLoads(this);
		}

        public override void Dispose()
        {
            UnregisterForMapBlockLoads(this);
            base.Dispose();
        }

        internal override void Draw(MapTile tile, Position3D position)
        {
            if (m_unloadedTiles.Count == 0)
                return;

            List<Point> drawnTiles = new List<Point>();

            foreach (Point p in m_unloadedTiles)
            {
                int x = tile.X + p.X - m_Components.Center.X;
                int y = tile.Y + p.Y - m_Components.Center.Y;

                MapTile t = Map.GetMapTile(x, y);
                if (t != null)
                {
                    drawnTiles.Add(p);

                    if (!m_hasCustomTiles)
                    {
                        if (p.X < m_Components.Width && p.Y < m_Components.Height)
                        {
                            foreach (StaticTile s in m_Components.Tiles[p.X][p.Y])
                            {
                                // t.AddMapObject(new MapObjectStatic(s.ID, 0, new Position3D(x, y, s.Z)));
                            }
                        }
                    }
                    else
                    {
                        foreach (StaticTile s in m_customHouseTiles)
                        {
                            if ((s.X == p.X) && (s.Y == p.Y))
                            {
                                // t.AddMapObject(new MapObjectStatic(s.ID, 0, new Position3D(s.X, s.Y, s.Z)));
                            }
                        }
                    }
                }
            }

            foreach (Point p in drawnTiles)
            {
                m_unloadedTiles.Remove(p);
            }
       }
    }
}
