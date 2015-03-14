/***************************************************************************
 *   MapTile.cs
 *   Based on code from ClintXNA's renderer.
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using UltimaXNA.Entity;
using Microsoft.Xna.Framework;
using UltimaXNA.UltimaWorld.View;
using UltimaXNA.UltimaData;
#endregion

namespace UltimaXNA.UltimaWorld
{
    public class MapTile
    {
        private bool m_NeedsSorting = false;

        private Ground m_Ground;
        public Ground Ground
        {
            get
            {
                return m_Ground;
            }
        }

        public int X
        {
            get { return m_Ground.X; }
        }

        public int Y
        {
            get { return m_Ground.Y; }
        }

        private List<BaseEntity> m_Entities;

        public void OnEnter(BaseEntity entity)
        {
            m_Entities.Add(entity);
            m_NeedsSorting = true;
        }

        public void OnExit(BaseEntity entity)
        {
            m_Entities.Remove(entity);
        }

        public MapTile(Ground ground)
        {
            m_Ground = ground;
            m_Entities = new List<BaseEntity>();
            m_Entities.Add(m_Ground);
        }

        /// <summary>
        /// Checks if the specified z-height is under an item or a ground.
        /// </summary>
        /// <param name="originZ"></param>
        /// <param name="underItem"></param>
        /// <param name="underTerrain"></param>
        public void IsPointUnderAnEntity(int originZ, out BaseEntity underItem, out BaseEntity underTerrain)
        {
            underItem = null;
            underTerrain = null;

            List<BaseEntity> iObjects = this.Items;
            for (int i = iObjects.Count - 1; i >= 0; i--)
            {
                if (iObjects[i].Z <= originZ)
                    continue;

                if (iObjects[i] is StaticItem)
                {
                    UltimaData.ItemData iData = ((StaticItem)iObjects[i]).ItemData;
                    if (iData.IsRoof || iData.IsSurface || iData.IsWall)
                    {
                        if (underItem == null || iObjects[i].Z < underItem.Z)
                            underItem = iObjects[i];
                    }
                }
                else if (iObjects[i] is Ground && iObjects[i].Z >= originZ + 20)
                {
                    underTerrain = iObjects[i];
                }
            }
        }

        public void RemoveEntity(Serial serial)
        {
            for (int i = 0; i < m_Entities.Count; i++)
            {
                if (m_Entities[i].Serial == serial)
                {
                    m_Entities.RemoveAt(i);
                    i--;
                }
            }
        }

        public List<StaticItem> GetStatics()
        {
            List<StaticItem> items = new List<StaticItem>();

            for (int i = 0; i < m_Entities.Count; i++)
            {
                if (m_Entities[i] is StaticItem)
                    items.Add((StaticItem)m_Entities[i]);
            }

            return items;
        }

        private bool matchNames(ItemData m1, ItemData m2)
        {
            return (m1.Name == m2.Name);
        }

        private void removeDuplicateObjects()
        {
            int[] itemsToRemove = new int[0x100];
            int removeIndex = 0;

            for (int i = 0; i < m_Entities.Count; i++)
            {
                for (int j = 0; j < removeIndex; j++)
                {
                    if (itemsToRemove[j] == i)
                        continue;
                }

                if (m_Entities[i] is StaticItem)
                {
                    // Make sure we don't double-add a static or replace an item with a static (like doors on multis)
                    for (int j = i + 1; j < m_Entities.Count; j++)
                    {
                        if (m_Entities[i].Z == m_Entities[j].Z)
                        {
                            if (m_Entities[j] is StaticItem && 
                                ((StaticItem)m_Entities[i]).ItemID == ((StaticItem)m_Entities[j]).ItemID ||
                                matchNames(((StaticItem)m_Entities[i]).ItemData, ((StaticItem)m_Entities[j]).ItemData))
                            {
                                itemsToRemove[removeIndex++] = i;
                                break;
                            }
                        }
                    }
                }
                else if (m_Entities[i] is Item)
                {
                    // if we are adding an item, replace existing statics with the same *name*
                    // We could use same *id*, but this is more robust for items that can open ...
                    // an open door will have a different id from a closed door, but the same name.
                    // Also, don't double add an item.
                    for (int j = i + 1; j < m_Entities.Count; j++)
                    {
                        if (m_Entities[i].Z == m_Entities[j].Z)
                        {
                            if ((m_Entities[j] is StaticItem && matchNames(((Item)m_Entities[i]).ItemData, ((StaticItem)m_Entities[j]).ItemData)) ||
                                (m_Entities[j] is Item && m_Entities[i].Serial == m_Entities[j].Serial))
                            {
                                itemsToRemove[removeIndex++] = j;
                                continue;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < removeIndex; i++)
            {
                m_Entities.RemoveAt(itemsToRemove[i] - i);
            }
        }

        public List<BaseEntity> Items
        {
            get
            {
                if (m_NeedsSorting)
                {
                    removeDuplicateObjects();
                    m_Entities.Sort(MapObjectComparer.Comparer);
                    m_NeedsSorting = false;
                }
                return m_Entities;
            }
        }

        public void Resort()
        {
            m_NeedsSorting = true;
        }

        private static bool IsGroundTile(object i)
        {
            Type t = typeof(MapObjectGround);
            return t == i.GetType() || i.GetType().IsSubclassOf(t);
        }

        private static bool IsMobile(object i)
        {
            Type t = typeof(MapObjectMobile);
            return t == i.GetType() || i.GetType().IsSubclassOf(t);
        }

        private static bool IsStaticItem(object i)
        {
            Type t = typeof(MapObjectStatic);
            return t == i.GetType() || i.GetType().IsSubclassOf(t);
        }

        private static bool IsGOTile(object i)
        {
            Type t = typeof(MapObjectItem);
            return t == i.GetType() || i.GetType().IsSubclassOf(t);
        }
    }
}
