/***************************************************************************
 *   EntityManager.cs
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
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Entities.Items.Containers;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Ultima.World.Maps;
#endregion

namespace UltimaXNA.Ultima.World.Managers
{
    class EntityManager
    {
        WorldModel m_Model;
        Dictionary<int, AEntity> m_Entities = new Dictionary<int, AEntity>();
        List<AEntity> m_Entities_Queued = new List<AEntity>();
        List<Serial> m_RetainedPlayerEntities = new List<Serial>();
        bool m_EntitiesCollectionIsLocked = false;
        List<int> m_SerialsToRemove = new List<int>();
        List<OrphanedItem> m_OrphanedItems = new List<OrphanedItem>();

        public EntityManager(WorldModel model)
        {
            m_Model = model;
        }

        public void Reset(bool clearPlayerEntity = false)
        {
            m_OrphanedItems.Clear();
            m_RetainedPlayerEntities.Clear();
            if (!clearPlayerEntity)
            {
                Mobile player = GetPlayerEntity();
                if (player != null)
                {
                    RetainPlayerEntities(player, m_RetainedPlayerEntities);
                }
            }
            foreach (AEntity e in m_Entities.Values)
            {
                if (!m_RetainedPlayerEntities.Contains(e.Serial))
                    e.Dispose();
            }
        }

        void RetainPlayerEntities(Mobile player, List<Serial> retained)
        {
            retained.Add(player.Serial);
            for (int i = (int)EquipLayer.FirstValid; i <= (int)EquipLayer.LastUserValid; i++)
            {
                AEntity e = player.Equipment[i];
                if (e != null && !e.IsDisposed)
                {
                    retained.Add(e.Serial);
                    if (e is Container)
                    {
                        RecursiveRetainPlayerEntities(e as Container, retained);
                    }
                }
            }
        }

        void RecursiveRetainPlayerEntities(Container container, List<Serial> retained)
        {
            foreach (AEntity e in container.Contents)
            {
                if (e != null && !e.IsDisposed)
                {
                    retained.Add(e.Serial);
                    if (e is Container)
                    {
                        RecursiveRetainPlayerEntities(e as Container, retained);
                    }
                }
            }
        }

        public Mobile GetPlayerEntity()
        {
            // This could be cached to save time.
            if (m_Entities.ContainsKey(WorldModel.PlayerSerial))
                return (Mobile)m_Entities[WorldModel.PlayerSerial];
            return null;
        }

        public void Update(double frameMS)
        {
            if (WorldModel.IsInWorld)
            {
                UpdateEntities(frameMS);
            }
        }

        void UpdateEntities(double frameMS)
        {
            // redirect any new entities to a queue while we are enumerating the collection.
            m_EntitiesCollectionIsLocked = true;

            Mobile player = GetPlayerEntity();
            if (player == null)
            {
                // wait for the server to send us an updated player entity.
            }
            else
            {
                // Update all other entities, disposing when they are out of range.
                Position3D player_position = player.DestinationPosition;
                foreach (KeyValuePair<int, AEntity> entity in m_Entities)
                {
                    entity.Value.Update(frameMS);
                    if (System.Math.Abs(player_position.X - entity.Value.Position.X) > (entity.Value.GetMaxUpdateRange()) ||
                        System.Math.Abs(player_position.Y - entity.Value.Position.Y) > (entity.Value.GetMaxUpdateRange()))
                    {
                        entity.Value.Dispose();
                    }
                    if (entity.Value.IsDisposed)
                        m_SerialsToRemove.Add(entity.Key);
                }
            }
            // Remove disposed entities
            foreach (int i in m_SerialsToRemove)
            {
                m_Entities.Remove(i);
            }
            m_SerialsToRemove.Clear();
            // stop redirecting new entities to the queue and add any queued entities to the main entity collection.
            m_EntitiesCollectionIsLocked = false;
            foreach (AEntity e in m_Entities_Queued)
                m_Entities.Add(e.Serial, e);
            m_Entities_Queued.Clear();
        }

        public Overhead AddOverhead(MessageTypes msgType, Serial serial, string text, int fontID, int hue, bool asUnicode)
        {
            if (m_Entities.ContainsKey(serial))
            {
                AEntity ownerEntity = m_Entities[serial];
                Overhead overhead = ownerEntity.AddOverhead(msgType, text, fontID, hue, asUnicode);
                return overhead;
            }
            return null;
        }

        public T GetObject<T>(Serial serial, bool create) where T : AEntity
        {
            T entity;
            // Check for existence in the collection.
            if (m_Entities.ContainsKey(serial))
            {
                // This object is in the m_entities collection. If it is being disposed, then we should complete disposal
                // of the object and then return a new object. If it is not being disposed, return the object in the collection.
                entity = (T)m_Entities[serial];
                if (entity.IsDisposed)
                {
                    m_Entities.Remove(serial);
                    if (create)
                    {
                        entity = InternalCreateEntity<T>(serial);
                        return (T)entity;
                    }
                    return null;
                }
                return (T)m_Entities[serial];
            }
            // No object with this Serial is in the collection. So we create a new one and return that, and hope that the server
            // will fill us in on the details of this object soon.
            if (create)
            {
                entity = InternalCreateEntity<T>(serial);
                return (T)entity;
            }
            return null;
        }

        T InternalCreateEntity<T>(Serial serial) where T : AEntity
        {
            var ctor = typeof(T).GetConstructor(new[] { typeof(Serial), typeof(Map) });
            AEntity e = (T)ctor.Invoke(new object[] { serial, m_Model.Map });
            if (e.Serial == WorldModel.PlayerSerial)
            {
                e.IsClientEntity = true;
            }
            if (e is Mobile)
            {
                for (int i = 0; i < m_OrphanedItems.Count; i++)
                {
                    if (m_OrphanedItems[i].ParentSerial == serial)
                    {
                        (e as Mobile).WearItem(m_OrphanedItems[i].Item, m_OrphanedItems[i].Layer);
                        m_OrphanedItems.RemoveAt(i--);
                    }
                }
            }
            // If the entities collection is locked, add the new entity to the queue. Otherwise 
            // add it directly to the main entity collection.
            if (m_EntitiesCollectionIsLocked)
                m_Entities_Queued.Add(e);
            else
                m_Entities.Add(e.Serial, e);
            return (T)e;
        }

        public void RemoveEntity(Serial serial)
        {
            if (m_Entities.ContainsKey(serial))
            {
                m_Entities[serial].Dispose();
            }
        }

        public void AddWornItem(Item item, byte layer, Serial parent)
        {
            Mobile m = WorldModel.Entities.GetObject<Mobile>(parent, false);
            if (m != null)
            {
                m.WearItem(item, layer);
            }
            else
            {
                m_OrphanedItems.Add(new OrphanedItem(item, layer, parent));
            }
        }

        struct OrphanedItem
        {
            public readonly byte Layer;
            public readonly Item Item;
            public readonly Serial ParentSerial;

            public OrphanedItem(Item item, byte layer, Serial parent)
            {
                Item = item;
                Layer = layer;
                ParentSerial = parent;
            }
        }
    }
}

