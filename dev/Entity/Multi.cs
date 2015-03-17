/***************************************************************************
 *   Multi.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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

namespace UltimaXNA.Entity
{
    class Multi : AEntity
    {
        MultiComponentList m_components;
        List<Point> m_unloadedTiles = new List<Point>();

        int m_customHouseRevision = 0x7FFFFFFF;
        StaticTile[] m_customHouseTiles;
        public int CustomHouseRevision { get { return m_customHouseRevision; } }

        bool m_hasCustomTiles = false;
        CustomHouse m_customHouse;
        public void AddCustomHousingTiles(CustomHouse house)
        {
            m_hasCustomTiles = true;
            m_customHouse = house;
            m_customHouseTiles = house.GetStatics(m_components.Width, m_components.Height);
            redrawAllTiles();
        }

        int m_ItemID;
        public int ItemID
        {
            get { return m_ItemID; }
            set
            {
                if (m_ItemID != value)
                {
                    m_ItemID = value;
                    redrawAllTiles();
                }
            }
        }

        void redrawAllTiles()
        {
            m_components = MultiData.GetComponents(m_ItemID);
            m_unloadedTiles.Clear();
            for (int y = 0; y < m_components.Height + 1; y++)
            {
                for (int x = 0; x < m_components.Width; x++)
                {
                    Point p = new Point();
                    p.X = x;
                    p.Y = y;
                    m_unloadedTiles.Add(p);
                }
            }
        }

        public Multi(Serial serial)
			: base(serial)
		{
		}

        public override void Update(double frameMS)
        {
            if (m_unloadedTiles.Count > 0)
            {
                // what do we do here ???
            }

            base.Update(frameMS);
        }

        internal override void Draw(MapTile tile, Position3D position)
        {
            if (m_unloadedTiles.Count == 0)
                return;

            List<Point> drawnTiles = new List<Point>();

            foreach (Point p in m_unloadedTiles)
            {
                int x = tile.X + p.X - m_components.Center.X;
                int y = tile.Y + p.Y - m_components.Center.Y;

                MapTile t = EntityManager.Model.Map.GetMapTile(x, y);
                if (t != null)
                {
                    drawnTiles.Add(p);

                    if (!m_hasCustomTiles)
                    {
                        if (p.X < m_components.Width && p.Y < m_components.Height)
                        {
                            foreach (StaticTile s in m_components.Tiles[p.X][p.Y])
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
