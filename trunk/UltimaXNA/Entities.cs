/***************************************************************************
 *   GameObjects.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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
using UltimaXNA.Core.Extensions;
using Microsoft.Xna.Framework;
using UltimaXNA.Network.Packets.Client;
using UltimaXNA.Interface.TileEngine;
using UltimaXNA.Entity;
#endregion

namespace UltimaXNA
{
    static class Entities
    {
        private static Dictionary<int, BaseEntity> _entities = new Dictionary<int, BaseEntity>();
        private static List<BaseEntity> _entities_Queued = new List<BaseEntity>();
        private static bool _entitiesCollectionIsLocked = false;

        public static int MySerial { get; set; }

        public static void Reset()
        {
            _entities.Clear();
        }

        public static BaseEntity GetPlayerObject()
        {
            // This could be cached to save time.
            if (_entities.ContainsKey(MySerial))
                return _entities[MySerial];
            else

                return null;
        }


        public static void Update(GameTime gameTime)
        {
            if (ClientVars.EngineVars.InWorld)
            {
                updateEntities(gameTime);
            }
        }

        static List<int> _entitiesToRemove = new List<int>();
        private static void updateEntities(GameTime gameTime)
        {
            // redirect any new entities to a queue while we are enumerating the collection.
            _entitiesCollectionIsLocked = true;

            // Get the player object
            BaseEntity player = GetPlayerObject();

            // Update the player entity first because we cull entities out of range of this main object.
            player.Update(gameTime);
            if (player.IsDisposed)
                _entitiesToRemove.Add(player.Serial);

            // Update all other entities.
            foreach (KeyValuePair<int, BaseEntity> entity in _entities)
            {
                // Don't update the player entity twice!
                if (entity.Key == MySerial)
                    continue;
                if (!entity.Value.IsDisposed)
                    entity.Value.Update(gameTime);
                // Dispose the entity if it is out of range.
                if (!Utility.InRange(entity.Value.WorldPosition, player.Position, ClientVars.EngineVars.UpdateRange))
                    entity.Value.Dispose();
                if (entity.Value.IsDisposed)
                    _entitiesToRemove.Add(entity.Key);
            }

            // Remove disposed entities
            foreach (int i in _entitiesToRemove)
            {
                _entities.Remove(i);
            }
            _entitiesToRemove.Clear();

            // stop redirecting new entities to the queue and add any queued entities to the main entity collection.
            _entitiesCollectionIsLocked = false;
            foreach (BaseEntity e in _entities_Queued)
                _entities.Add(e.Serial, e);
            _entities_Queued.Clear();
        }

        public static Overhead AddOverhead(MessageType msgType, Serial serial, string text, int fontID, int hue)
        {
            if (_entities.ContainsKey(serial))
            {
                BaseEntity ownerEntity = _entities[serial];
                Overhead overhead = ownerEntity.AddOverhead(msgType, text, fontID, hue);
                return overhead;
            }
            else
            {
                return null;
            }
        }

        public static DynamicObject AddDynamicObject()
        {
            DynamicObject dynamic = addObject<DynamicObject>(Serial.NewDynamicSerial);
            return dynamic;
        }

        public static List<T> GetObjectsByType<T>() where T : BaseEntity
        {
            List<T> list = new List<T>();
            foreach (BaseEntity e in _entities.Values)
            {
                if (e is T)
                {
                    T typedEntity = (T)e;
                    list.Add(typedEntity);
                }
            }
            return list;
        }

        public static T GetObject<T>(Serial serial, bool create) where T : BaseEntity
        {
            T entity;
            // Check for existence in the collection.
            if (_entities.ContainsKey(serial))
            {
                // This object is in the _entities collection. If it is being disposed, then we should complete disposal
                // of the object and then return a new object. If it is not being disposed, return the object in the collection.
                if (_entities[serial].IsDisposed)
                {
                    if (create)
                    {
                        _entities.Remove(serial);
                        entity = addObject<T>(serial);
                        return (T)entity;
                    }
                    else
                    {
                        return null;
                    }
                }
                return (T)_entities[serial];
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

        static T addObject<T>(Serial serial) where T : BaseEntity
        {
            BaseEntity e;
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
                case "DynamicObject":
                    e = new DynamicObject(serial);
                    break;
                default:
                    throw new Exception("Unknown addObject type!");
            }

            if (e.Serial == MySerial)
                e.IsClientEntity = true;

            // If the entities collection is locked, add the new entity to the queue. Otherwise 
            // add it directly to the main entity collection.
            if (_entitiesCollectionIsLocked)
                _entities_Queued.Add(e);
            else
                _entities.Add(e.Serial, e);

            return (T)e;
        }

        public static void RemoveObject(Serial serial)
        {
            if (_entities.ContainsKey(serial))
            {
                _entities[serial].Dispose();
            }
        }

        
    }
}
