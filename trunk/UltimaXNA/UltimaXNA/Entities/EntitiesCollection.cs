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
using UltimaXNA.Network.Packets.Client;
using UltimaXNA.TileEngine;
#endregion

namespace UltimaXNA.Entities
{
    static class EntitiesCollection
    {
        private static Dictionary<int, Entity> _entities = new Dictionary<int, Entity>();
        private static IWorld _world;
        public static int MySerial { get; set; }

        static EntitiesCollection()
        {
        }

        public static void Initialize(Game game)
        {
            _world = game.Services.GetService<IWorld>();
        }

        public static void Reset()
        {
            _entities.Clear();
        }

        public static Entity GetPlayerObject()
        {
            // This could be cached to save time.
            if (_entities.ContainsKey(MySerial))
                return _entities[MySerial];
            else

                return null;
        }


        static List<int> _removeObjectsList = new List<int>();
        public static void Update(GameTime gameTime)
        {
            if (ClientVars.InWorld)
            {
                // Clear the list of objects to be removed.
                _removeObjectsList.Clear();

                // Update the player entity first because we cull entities out of range of this main object.
                Entity player = GetPlayerObject();
                player.Update(gameTime);
                if (player.IsDisposed)
                    _removeObjectsList.Add(player.Serial);

                // Now update all other entities.
                foreach (KeyValuePair<int, Entity> entity in _entities)
                {
                    // Don't update the player entity twice!
                    if (entity.Key == MySerial)
                        continue;
                    entity.Value.Update(gameTime);
                    // Dispose the entity if it is out of range.
                    if (!Utility.InRange(entity.Value.WorldPosition, player.Position, ClientVars.UpdateRange))
                        entity.Value.Dispose();
                    if (entity.Value.IsDisposed)
                        _removeObjectsList.Add(entity.Key);
                }
                foreach (int i in _removeObjectsList)
                {
                    _entities.Remove(i);
                }
            }
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
                // This object is in the m_Objects collection. If it is being disposed, then we should complete disposal
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
            /*
            T o = (T)Activator.CreateInstance(typeof(T), new object[] { serial, _world });
            // If this object is the client, designate it to return events.
            if (o.Serial == MySerial)
                o.IsClientEntity = true;
            _entities.Add(o.Serial, o); // Add the object to the objects collection.
            return (T)o;
            */
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
            // When Dispose() is called, the object will tidy up and then
            // set m_Dispose = true. Reference this with IsDisposed on the
            // next update cycle.
            if (_entities.ContainsKey(serial))
            {
                _entities[serial].Dispose();
            }
        }

        
    }
}
