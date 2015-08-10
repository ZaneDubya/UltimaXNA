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
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Ultima.World.Maps;
#endregion

namespace UltimaXNA.Ultima.World.Managers
{
    class EntityManager
    {
        private WorldModel m_Model;

        private Dictionary<int, AEntity> m_Entities = new Dictionary<int, AEntity>();
        private List<AEntity> m_Entities_Queued = new List<AEntity>();
        private bool m_EntitiesCollectionIsLocked = false;
        List<int> m_SerialsToRemove = new List<int>();

        public EntityManager(WorldModel model)
        {
            m_Model = model;
        }

        public void Reset(bool clearPlayerEntity = false)
        {
            if (clearPlayerEntity)
            {
                m_Entities.Clear();
                foreach (AEntity entity in m_Entities.Values)
                {
                    entity.Dispose();
                }
            }
            else
            {
                foreach (AEntity entity in m_Entities.Values)
                {
                    if (!entity.IsClientEntity)
                        entity.Dispose();
                }
                AEntity player = GetPlayerEntity();
                m_Entities.Clear();
                if (player != null)
                    m_Entities.Add(player.Serial, player);
            }
        }

        public Mobile GetPlayerEntity()
        {
            // This could be cached to save time.
            if (m_Entities.ContainsKey(WorldModel.PlayerSerial))
                return (Mobile)m_Entities[WorldModel.PlayerSerial];
            else
                return null;
        }

        public void Update(double frameMS)
        {
            if (WorldModel.IsInWorld)
            {
                updateEntities(frameMS);
            }
        }

        private void updateEntities(double frameMS)
        {
            // redirect any new entities to a queue while we are enumerating the collection.
            m_EntitiesCollectionIsLocked = true;

            // Update all other entities.
            foreach (KeyValuePair<int, AEntity> entity in m_Entities)
            {
                if (!entity.Value.IsDisposed)
                    entity.Value.Update(frameMS);
                else
                    m_SerialsToRemove.Add(entity.Key);
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

        public Overhead AddOverhead(MessageTypes msgType, Serial serial, string text, int fontID, int hue)
        {
            if (m_Entities.ContainsKey(serial))
            {
                AEntity ownerEntity = m_Entities[serial];
                Overhead overhead = ownerEntity.AddOverhead(msgType, text, fontID, hue);
                return overhead;
            }
            else
            {
                return null;
            }
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
                    else
                    {
                        return null;
                    }
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
            else
            {
                return null;
            }
        }

        T InternalCreateEntity<T>(Serial serial) where T : AEntity
        {
            var ctor = typeof(T).GetConstructor(new[] { typeof(Serial), typeof(Map) });

            AEntity e = (T)ctor.Invoke(new object[] { serial, m_Model.Map });

            if (e.Serial == WorldModel.PlayerSerial)
                e.IsClientEntity = true;

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
    }
}
