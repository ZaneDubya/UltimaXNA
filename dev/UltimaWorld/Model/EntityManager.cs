/***************************************************************************
 *   EntityManager.cs
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
using Microsoft.Xna.Framework;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaWorld;
using UltimaXNA.Entity;
#endregion

namespace UltimaXNA.UltimaWorld
{
    class EntityManager
    {
        private static WorldModel m_Model;
        public static WorldModel Model
        {
            get { return m_Model; }
        }

        private static Dictionary<int, AEntity> m_Entities = new Dictionary<int, AEntity>();
        private static List<AEntity> m_Entities_Queued = new List<AEntity>();
        private static bool m_EntitiesCollectionIsLocked = false;
        static List<int> m_SerialsToRemove = new List<int>();

        public static int MySerial { get; set; }

        public EntityManager(WorldModel model)
        {
            m_Model = model;
        }

        public static void Reset()
        {
            m_Entities.Clear();
        }

        public static AEntity GetPlayerObject()
        {
            // This could be cached to save time.
            if (m_Entities.ContainsKey(MySerial))
                return m_Entities[MySerial];
            else
                return null;
        }

        public static void Update(double frameMS)
        {
            if (UltimaVars.EngineVars.InWorld)
            {
                updateEntities(frameMS);
            }
        }

        private static void updateEntities(double frameMS)
        {
            // redirect any new entities to a queue while we are enumerating the collection.
            m_EntitiesCollectionIsLocked = true;

            // Get the player object
            AEntity player = GetPlayerObject();

            // Update the player entity first because we cull entities out of range of this main object.
            player.Update(frameMS);
            if (player.IsDisposed)
                m_SerialsToRemove.Add(player.Serial);

            // Update all other entities.
            foreach (KeyValuePair<int, AEntity> entity in m_Entities)
            {
                // Don't update the player entity twice!
                if (entity.Key == MySerial)
                    continue;
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

        public static Overhead AddOverhead(MessageType msgType, Serial serial, string text, int fontID, int hue)
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

        public static Effect AddDynamicObject()
        {
            Effect dynamic = addObject<Effect>(Serial.NewDynamicSerial);
            return dynamic;
        }

        public static List<T> GetObjectsByType<T>() where T : AEntity
        {
            List<T> list = new List<T>();
            foreach (AEntity e in m_Entities.Values)
            {
                if (e is T)
                {
                    T typedEntity = (T)e;
                    list.Add(typedEntity);
                }
            }
            return list;
        }

        public static T GetObject<T>(Serial serial, bool create) where T : AEntity
        {
            T entity;
            // Check for existence in the collection.
            if (m_Entities.ContainsKey(serial))
            {
                // This object is in the m_entities collection. If it is being disposed, then we should complete disposal
                // of the object and then return a new object. If it is not being disposed, return the object in the collection.
                if (m_Entities[serial].IsDisposed)
                {
                    if (create)
                    {
                        m_Entities.Remove(serial);
                        entity = addObject<T>(serial);
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
                entity = addObject<T>(serial);
                return (T)entity;
            }
            else
            {
                return null;
            }
        }

        static T addObject<T>(Serial serial) where T : AEntity
        {
            AEntity e;
            Type t = typeof(T);
            switch (t.Name)
            {
                case "Item":
                    e = new Item(serial);
                    break;
                case "Container":
                    e = new Container(serial);
                    break;
                case "Mobile":
                    e = new Mobile(serial);
                    break;
                case "PlayerMobile":
                    e = new PlayerMobile(serial);
                    break;
                case "Corpse":
                    e = new Corpse(serial);
                    break;
                case "Multi":
                    e = new Multi(serial);
                    break;
                case "Effect":
                    e = new Effect(serial);
                    break;
                default:
                    throw new Exception("Unknown addObject type!");
            }

            if (e.Serial == MySerial)
                e.IsClientEntity = true;

            // If the entities collection is locked, add the new entity to the queue. Otherwise 
            // add it directly to the main entity collection.
            if (m_EntitiesCollectionIsLocked)
                m_Entities_Queued.Add(e);
            else
                m_Entities.Add(e.Serial, e);

            return (T)e;
        }

        public static void RemoveObject(Serial serial)
        {
            if (m_Entities.ContainsKey(serial))
            {
                m_Entities[serial].Dispose();
            }
        }
    }
}
