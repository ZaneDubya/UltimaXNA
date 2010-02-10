/***************************************************************************
 *   MapCell.cs
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
using System;
using System.Collections.Generic;
using UltimaXNA.Entities;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.TileEngine
{
    public sealed class MapTile : IPoint2D
    {
        public List<MapObject> Objects { get { return m_Objects; } }
        private List<MapObject> m_Objects;

        private bool m_NeedsSorting;
        private static int[] m_itemsToRemove = new int[0x100];



        #region X
        private int m_X;
        public int X
        {
            get { return m_X; }
            set { m_X = value; }
        }
        #endregion
        #region Y
        private int m_Y;
        public int Y
        {
            get { return m_Y; }
            set { m_Y = value; }
        }
        #endregion

        public MapTile(int x, int y)
        {
            m_Objects = new List<MapObject>();
            m_X = x;
            m_Y = y;
        }

        // Check if under a roof. --Poplicola 6/2/2009.
        public void IsUnder(int nAltitude, out bool isUnderItem, out bool isUnderTerrain)
        {
            isUnderItem = false;
            isUnderTerrain = false;

            List<MapObject> iObjects = this.GetSortedObjects();
            for (int i = iObjects.Count - 1; i >= 0; i--)
            {
                if (iObjects[i].Z <= nAltitude)
                    continue;

                if (iObjects[i] is MapObjectStatic)
                {
                    Data.ItemData iData = Data.TileData.ItemData[((MapObjectStatic)iObjects[i]).ItemID & 0x3FFF];
                    if (iData.Roof || iData.Surface)
                        isUnderItem = true;
                }
                else if (iObjects[i] is MapObjectGround && iObjects[i].Z >= nAltitude + 20)
                {
                    isUnderTerrain = true;
                }
            }
        }

        // Poplicola 5/14/2009.
        public void FlushObjectsBySerial(Serial serial)
        {
            List<MapObject> iObjects = new List<MapObject>();
            foreach (MapObject iObject in m_Objects)
            {
                if (iObject.OwnerSerial == serial)
                {
                    // Do nothing. Object is skipped.
                }
                else
                {
                    iObjects.Add(iObject);
                }
                m_Objects = iObjects;
                m_NeedsSorting = true;
            }
            m_NeedsSorting = true;
        }

        // Poplicola 5/9/2009
        public MapObjectGround GroundTile
        {
            get
            {
                return (MapObjectGround)m_Objects.Find(IsGroundTile);
            }

        }
        // Poplicola 5/9/2009
        private static bool IsGroundTile(object i)
        {
            Type t = typeof(MapObjectGround);
            return t == i.GetType() || i.GetType().IsSubclassOf(t);
        }
        // Poplicola 5/10/2009
        private static bool IsMobile(object i)
        {
            Type t = typeof(MapObjectMobile);
            return t == i.GetType() || i.GetType().IsSubclassOf(t);
        }
        // Issue 5 - Statics (bridge, stairs, etc) should be walkable - http://code.google.com/p/ultimaxna/issues/detail?id=5 - Smjert
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

        public List<MapObjectStatic> GetStatics()
        {
            List<MapObjectStatic> sitems = new List<MapObjectStatic>();

            List<MapObject> objs = m_Objects.FindAll(IsStaticItem);
            if (objs == null || objs.Count == 0)
            {
                // empty list.
                return sitems;
            }

            foreach (MapObject obj in objs)
            {
                sitems.Add((MapObjectStatic)obj);
            }

            return sitems;
        }

        private bool matchNames(MapObject m1, MapObject m2)
        {
            if (Data.TileData.ItemData[m1.ItemID & 0x3FFF].Name ==
                Data.TileData.ItemData[m2.ItemID & 0x3FFF].Name)
            {
                return true;
            }
            return false;
        }

        public void Add(MapObject item)
        {
            m_Objects.Add(item);
            m_NeedsSorting = true;
        }

        public void Add(MapObject[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                Add(items[i]);
            }
        }

        public List<MapObject> GetSortedObjects()
        {
            if (m_NeedsSorting)
            {
                removeDuplicateObjects();
                m_Objects.Sort(TileComparer.Comparer);
                m_NeedsSorting = false;
            }
            return m_Objects;
        }

        private void removeDuplicateObjects()
        {
            
            int removeIndex = 0;

            for (int i = 0; i < m_Objects.Count; i++)
            {
                for (int j = 0; j < removeIndex; j++)
                {
                    if (m_itemsToRemove[j] == i)
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
                                m_itemsToRemove[removeIndex++] = i;
                                break;
                            }
                            if (m_Objects[j] is MapObjectItem && matchNames(m_Objects[i], m_Objects[j]))
                            {
                                m_itemsToRemove[removeIndex++] = i;
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
                                m_itemsToRemove[removeIndex++] = j;
                                continue;
                            }
                            if (m_Objects[j] is MapObjectItem && m_Objects[i].OwnerEntity == m_Objects[j].OwnerEntity)
                            {
                                m_itemsToRemove[removeIndex++] = i;
                                break;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < removeIndex; i++)
            {
                m_Objects.RemoveAt(m_itemsToRemove[i] - i);
            }
        }

        public List<MapObject> Items
        {
            get
            {
                return new List<MapObject>(this.GetSortedObjects());
            }
        }
    }
}
