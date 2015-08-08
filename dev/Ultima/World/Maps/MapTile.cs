/***************************************************************************
 *   MapTile.cs
 *   Based on code from ClintXNA.
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Entities.Items;
#endregion

namespace UltimaXNA.Ultima.World.Maps
{
    /// <summary>
    /// Represents a single tile on the Ultima Online map.
    /// </summary>
    public class MapTile
    {
        public MapTile()
        {
            m_Entities = new List<AEntity>();
        }

        /// <summary>
        /// The Ground entity for this tile. Every tile has one and only one ground entity.
        /// </summary>
        public Ground Ground
        {
            get;
            private set;
        }

        public int X
        {
            get { return Ground.Position.X; }
        }

        public int Y
        {
            get { return Ground.Position.Y; }
        }

        // ============================================================
        // Entity management
        // ============================================================

        private List<AEntity> m_Entities;

        /// <summary>
        /// Adds the passed entity to this Tile's entity collection, and forces a resort of the entities on this tile.
        /// </summary>
        /// <param name="entity"></param>
        public void OnEnter(AEntity entity)
        {
            // only allow one ground object.
            if (entity is Ground)
            {
                if (Ground != null)
                    Ground.Dispose();
                Ground = (Ground)entity;
            }

            // if we are receiving a Item with the same position and itemID as a static item, then replace the static item.
            if (entity is Item)
            {
                Item item = entity as Item;
                for (int i = 0; i < m_Entities.Count; i++)
                {
                    if (m_Entities[i] is Item)
                    {
                        Item comparison = m_Entities[i] as Item;
                        if (comparison.ItemID == item.ItemID &&
                            comparison.Z == item.Z)
                        {
                            m_Entities.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }

            m_Entities.Add(entity);
            m_NeedsSorting = true;
        }

        public bool ItemExists(int itemID, int z)
        {
            for (int i = 0; i < m_Entities.Count; i++)
            {
                if (m_Entities[i] is Item)
                {
                    Item comparison = m_Entities[i] as Item;
                    if (comparison.ItemID == itemID &&
                        comparison.Z == z)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the passed entity from this Tile's entity collection.
        /// </summary>
        /// <param name="entity"></param>
        public void OnExit(AEntity entity)
        {
            m_Entities.Remove(entity);
        }

        /// <summary>
        /// Checks if the specified z-height is under an item or a ground.
        /// </summary>
        /// <param name="z">The z value to check.</param>
        /// <param name="underEntity">Returns the first roof, surface, or wall that is over the specified z.
        ///                         If no such objects exist above the specified z, then returns null.</param>
        /// <param name="underGround">Returns the ground object of this tile if the specified z is under the ground.
        ///                         Returns null otherwise.</param>
        public void IsZUnderEntityOrGround(int z, out AEntity underEntity, out AEntity underGround)
        {
            // getting the publicly exposed Entities collection will sort the entities if necessary.
            List<AEntity> entities = Entities;

            underEntity = null;
            underGround = null;

            for (int i = entities.Count - 1; i >= 0; i--)
            {
                if (entities[i].Z <= z)
                    continue;

                if (entities[i] is Item) // checks Item and StaticItem entities.
                {
                    ItemData data = ((Item)entities[i]).ItemData;
                    if (data.IsRoof || data.IsSurface || (data.IsWall && data.IsImpassable))
                    {
                        if (underEntity == null || entities[i].Z < underEntity.Z)
                            underEntity = entities[i];
                    }
                }
                else if (entities[i] is Ground && entities[i].GetView().SortZ >= z + 12)
                {
                    underGround = entities[i];
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

        private void InternalRemoveDuplicateEntities()
        {
            int[] itemsToRemove = new int[0x100];
            int removeIndex = 0;

            for (int i = 0; i < m_Entities.Count; i++)
            {
                // !!! TODO: I think this is wrong...
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
                            if (m_Entities[j] is StaticItem && (
                                ((StaticItem)m_Entities[i]).ItemID == ((StaticItem)m_Entities[j]).ItemID))
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

        public List<AEntity> Entities
        {
            get
            {
                if (m_NeedsSorting)
                {
                    InternalRemoveDuplicateEntities();
                    TileSorter.Sort(m_Entities);
                    m_NeedsSorting = false;
                }
                return m_Entities;
            }
        }

        // ============================================================
        // Sorting
        // ============================================================

        private bool m_NeedsSorting;

        public void ForceSort()
        {
            m_NeedsSorting = true;
        }
    }
}
