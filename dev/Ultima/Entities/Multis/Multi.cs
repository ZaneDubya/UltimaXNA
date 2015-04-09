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
using UltimaXNA.UltimaWorld.Views;
using Microsoft.Xna.Framework;
using UltimaXNA.UltimaWorld.Maps;
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
                    s_RegisteredMultis[i].PlaceTilesIntoNewlyLoadedBlock(block);
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

                MapTile tile = Map.GetMapTile(x, y);
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

        private void PlaceTilesIntoNewlyLoadedBlock(MapBlock block)
        {
            int px = Position.X;
            int py = Position.Y;

            Rectangle bounds = new Rectangle(block.X * 8, block.Y * 8, 8, 8);

            foreach (MultiComponentList.MultiItem item in m_Components.Items)
            {
                int x = px + item.OffsetX;
                int y = py + item.OffsetY;

                if (bounds.Contains(x, y))
                {
                    // would it be faster to get the tile from the block?
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
