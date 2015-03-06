/***************************************************************************
 *   MapCell.cs
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
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
#endregion

namespace UltimaXNA.UltimaWorld
{
    public sealed class MapTile : IPoint2D
    {
        private List<AMapObject> m_Objects;

        private bool m_NeedsSorting = false;
        private bool m_HasDeferredObjects = false;

        private int m_X, m_Y;
        public int X
        {
            get { return m_X; }
            set { m_X = value; }
        }
        public int Y
        {
            get { return m_Y; }
            set { m_Y = value; }
        }

        public MapTile(int x, int y)
        {
            m_Objects = new List<AMapObject>();
            m_X = x;
            m_Y = y;
        }

        // Check if under a roof.
        public void IsUnder(int originZ, out AMapObject underItem, out AMapObject underTerrain)
        {
            underItem = null;
            underTerrain = null;

            List<AMapObject> iObjects = this.Items;
            for (int i = iObjects.Count - 1; i >= 0; i--)
            {
                if (iObjects[i].Z <= originZ)
                    continue;

                if (iObjects[i] is MapObjectStatic)
                {
                    UltimaData.ItemData iData = UltimaData.TileData.ItemData[((MapObjectStatic)iObjects[i]).ItemID & 0x3FFF];
                    if (iData.Roof || iData.Surface || iData.Wall)
                    {
                        if (underItem == null || iObjects[i].Z < underItem.Z)
                            underItem = iObjects[i];
                    }
                }
                else if (iObjects[i] is MapObjectGround && iObjects[i].Z >= originZ + 20)
                {
                    underTerrain = iObjects[i];
                }
            }
        }

        public void FlushObjectsBySerial(Serial serial)
        {
            List<AMapObject> iObjects = new List<AMapObject>();
            foreach (AMapObject iObject in m_Objects)
            {
                if (iObject.OwnerSerial == serial)
                {
                    // Do nothing. Object is skipped.
                }
                else
                {
                    iObjects.Add(iObject);
                }
            }
            m_Objects = iObjects;
            m_NeedsSorting = true;
        }

        public MapObjectGround GroundTile
        {
            get
            {
                return (MapObjectGround)m_Objects.Find(IsGroundTile);
            }

        }

        public List<MapObjectStatic> GetStatics()
        {
            List<MapObjectStatic> sitems = new List<MapObjectStatic>();

            List<AMapObject> objs = m_Objects.FindAll(IsStaticItem);
            if (objs == null || objs.Count == 0)
            {
                // empty list.
                return sitems;
            }

            foreach (AMapObject obj in objs)
            {
                sitems.Add((MapObjectStatic)obj);
            }

            return sitems;
        }

        private bool matchNames(AMapObject m1, AMapObject m2)
        {
            if (UltimaData.TileData.ItemData[m1.ItemID & 0x3FFF].Name ==
                UltimaData.TileData.ItemData[m2.ItemID & 0x3FFF].Name)
            {
                return true;
            }
            return false;
        }

        public void AddMapObject(AMapObject item)
        {
            m_Objects.Add(item);
            m_NeedsSorting = true;
            if (item is MapObjectDeferred)
                m_HasDeferredObjects = true;
        }

        public void AddMapObject(AMapObject[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                AddMapObject(items[i]);
            }
        }

        private void removeDuplicateObjects()
        {
            int[] itemsToRemove = new int[0x100];
            int removeIndex = 0;

            for (int i = 0; i < m_Objects.Count; i++)
            {
                for (int j = 0; j < removeIndex; j++)
                {
                    if (itemsToRemove[j] == i)
                        continue;
                }

                if (m_Objects[i] is MapObjectStatic)
                {
                    // Make sure we don't double-add a static or replace an item with a static (like doors on multis)
                    for (int j = i + 1; j < m_Objects.Count; j++)
                    {
                        if (m_Objects[i].Z == m_Objects[j].Z)
                        {
                            if (m_Objects[j] is MapObjectStatic && m_Objects[i].ItemID == m_Objects[j].ItemID)
                            {
                                itemsToRemove[removeIndex++] = i;
                                break;
                            }
                            if (m_Objects[j] is MapObjectItem && matchNames(m_Objects[i], m_Objects[j]))
                            {
                                itemsToRemove[removeIndex++] = i;
                                break;
                            }
                        }
                    }
                }
                else if (m_Objects[i] is MapObjectItem)
                {
                    // if we are adding an item, replace existing statics with the same *name*
                    // We could use same *id*, but this is more robust for items that can open ...
                    // an open door will have a different id from a closed door, but the same name.
                    // Also, don't double add an item.
                    for (int j = i + 1; j < m_Objects.Count; j++)
                    {
                        if (m_Objects[i].Z == m_Objects[j].Z)
                        {
                            if (m_Objects[j] is MapObjectStatic && matchNames(m_Objects[i], m_Objects[j]))
                            {
                                itemsToRemove[removeIndex++] = j;
                                continue;
                            }
                            if (m_Objects[j] is MapObjectItem && m_Objects[i].OwnerEntity == m_Objects[j].OwnerEntity)
                            {
                                itemsToRemove[removeIndex++] = i;
                                break;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < removeIndex; i++)
            {
                m_Objects.RemoveAt(itemsToRemove[i] - i);
            }
        }

        public List<AMapObject> Items
        {
            get
            {
                if (m_NeedsSorting)
                {
                    removeDuplicateObjects();
                    m_Objects.Sort(MapObjectComparer.Comparer);
                    m_NeedsSorting = false;
                }
                return m_Objects;
            }
        }

        public void Resort()
        {
            m_NeedsSorting = true;
        }

        public void ClearTemporaryObjects()
        {
            if (m_HasDeferredObjects)
            {
                for (int i = 0; i < m_Objects.Count; i++)
                    if (m_Objects[i] is MapObjectDeferred)
                    {
                        ((MapObjectDeferred)m_Objects[i]).Dispose();
                        m_Objects.RemoveAt(i);
                        i--;
                    }
                m_HasDeferredObjects = false;
                Resort();
            }
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
