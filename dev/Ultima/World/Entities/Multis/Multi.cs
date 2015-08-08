/***************************************************************************
 *   Multi.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Maps;
#endregion

namespace UltimaXNA.Ultima.World.Entities.Multis
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

        public static void AnnounceMapChunkLoaded(MapChunk chunk)
        {
            for (int i = 0; i < s_RegisteredMultis.Count; i++)
                if (!s_RegisteredMultis[i].IsDisposed)
                    s_RegisteredMultis[i].PlaceTilesIntoNewlyLoadedChunk(chunk);
        }

        MultiComponentList m_Components;

        int m_customHouseRevision = 0x7FFFFFFF;
        StaticTile[] m_customHouseTiles;
        public int CustomHouseRevision { get { return m_customHouseRevision; } }

        // bool m_hasCustomTiles = false;
        CustomHouse m_customHouse;
        public void AddCustomHousingTiles(CustomHouse house)
        {
            // m_hasCustomTiles = true;
            m_customHouse = house;
            m_customHouseTiles = house.GetStatics(m_Components.Width, m_Components.Height);
        }

        int m_MultiID = -1;
        public int MultiID
        {
            get { return m_MultiID; }
            set
            {
                if (m_MultiID != value)
                {
                    m_MultiID = value;
                    m_Components = MultiData.GetComponents(m_MultiID);
                    InitialLoadTiles();
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

        private void InitialLoadTiles()
        {
            int px = Position.X;
            int py = Position.Y;

            foreach (MultiComponentList.MultiItem item in m_Components.Items)
            {
                int x = px + item.OffsetX;
                int y = py + item.OffsetY;

                MapTile tile = Map.GetMapTile((uint)x, (uint)y);
                if (tile != null)
                {
                    if (tile.ItemExists(item.ItemID, item.OffsetZ))
                        continue;

                    StaticItem staticItem = new StaticItem(item.ItemID, 0, 0, Map);
                    if (staticItem.ItemData.IsDoor)
                        continue;
                    staticItem.Position.Set(x, y, Z + item.OffsetZ);
                }
            }
        }

        private void PlaceTilesIntoNewlyLoadedChunk(MapChunk chunk)
        {
            int px = Position.X;
            int py = Position.Y;

            Rectangle bounds = new Rectangle((int)chunk.ChunkX * 8, (int)chunk.ChunkY * 8, 8, 8);

            foreach (MultiComponentList.MultiItem item in m_Components.Items)
            {
                int x = px + item.OffsetX;
                int y = py + item.OffsetY;

                if (bounds.Contains(x, y))
                {
                    // would it be faster to get the tile from the chunk?
                    MapTile tile = Map.GetMapTile(x, y);
                    if (tile != null)
                    {
                        if (!tile.ItemExists(item.ItemID, item.OffsetZ))
                        {
                            StaticItem staticItem = new StaticItem(item.ItemID, 0, 0, Map);
                            staticItem.Position.Set(x, y, Z + item.OffsetZ);
                        }
                    }
                }
            }
        }
    }
}
