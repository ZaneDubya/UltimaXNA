/***************************************************************************
 *   GameObjects.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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
using UltimaXNA.Extensions;
using Microsoft.Xna.Framework;
using UltimaXNA.Client.Packets.Client;
using UltimaXNA.TileEngine;
#endregion

namespace UltimaXNA.Entities
{
    static class EntitiesCollection
    {
        private static Dictionary<int, Entity> _entities = new Dictionary<int, Entity>();
        private static Dictionary<int, Entity> _dynamics = new Dictionary<int, Entity>();

        private static IIsometricRenderer _world;
        public static int MySerial { get; set; }

        static EntitiesCollection()
        {
        }

        public static void Initialize(Game game)
        {
            _world = game.Services.GetService<IIsometricRenderer>();
        }

        public static void Reset()
        {
            _entities.Clear();
            _dynamics.Clear();
        }

        public static Entity GetPlayerObject()
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
                updateDynamics(gameTime);
            }
        }

        static List<int> _entitiesToRemove = new List<int>();
        private static void updateEntities(GameTime gameTime)
        {
            // Get the player object
            Entity player = GetPlayerObject();

            // Update the player entity first because we cull entities out of range of this main object.
            player.Update(gameTime);
            if (player.IsDisposed)
                _entitiesToRemove.Add(player.Serial);

            // Update all other entities.
            foreach (KeyValuePair<int, Entity> entity in _entities)
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
        }

        private static void updateDynamics(GameTime gameTime)
        {
            // Get the player object
            Entity player = GetPlayerObject();

            // Update the dynamic objects
            foreach (KeyValuePair<int, Entity> dynamic in _dynamics)
            {
                if (!dynamic.Value.IsDisposed)
                    dynamic.Value.Update(gameTime);
                // Dispose the dynamic if it is out of range.
                if (!Utility.InRange(dynamic.Value.WorldPosition, player.Position, ClientVars.EngineVars.UpdateRange))
                    dynamic.Value.Dispose();
                if (dynamic.Value.IsDisposed)
                    _entitiesToRemove.Add(dynamic.Key);
            }

            // Remove disposed dynamics
            foreach (int i in _entitiesToRemove)
            {
                _dynamics.Remove(i);
            }
            _entitiesToRemove.Clear();
        }

        public static Overhead AddOverhead(MessageType msgType, Serial serial, string text, int fontID, int hue)
        {
            if (_entities.ContainsKey(serial))
            {
                Entity ownerEntity = _entities[serial];
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
            DynamicObject dynamic = new DynamicObject(_world);
            _dynamics.Add(dynamic.Serial, dynamic);
            return dynamic;
        }

        public static List<T> GetObjectsByType<T>() where T : Entity
        {
            List<T> list = new List<T>();
            foreach (Entity e in _entities.Values)
            {
                if (e is T)
                {
                    T typedEntity = (T)e;
                    list.Add(typedEntity);
                }
            }
            return list;
        }

        public static T GetObject<T>(Serial serial, bool create) where T : Entity
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

        static T addObject<T>(Serial serial) where T : Entity
        {
            Entity e;
            Type t = typeof(T);
            switch (t.Name)
            {
                case "Item":
                    e = new Item(serial, _world);
                    break;
                case "Container":
                    e = new Container(serial, _world);
                    break;
                case "Mobile":
                    e = new Mobile(serial, _world);
                    break;
                case "PlayerMobile":
                    e = new PlayerMobile(serial, _world);
                    break;
                case "Corpse":
                    e = new Corpse(serial, _world);
                    break;
                case "Multi":
                    e = new Multi(serial, _world);
                    break;
                default:
                    throw new Exception("Unknown addObject type!");
            }

            if (e.Serial == MySerial)
                e.IsClientEntity = true;
            _entities.Add(e.Serial, e); // Add the object to the objects collection.
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
