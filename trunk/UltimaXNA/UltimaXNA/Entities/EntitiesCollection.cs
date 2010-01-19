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

        public static void Update(GameTime gameTime)
        {
            if (ClientVars.InWorld)
            {
                List<int> removeObjectsList = removeObjectsList = new List<int>();
                foreach (KeyValuePair<int, Entity> entity in _entities)
                {
                    if (entity.Value.IsDisposed)
                    {
                        removeObjectsList.Add(entity.Key);
                        continue;
                    }
                    entity.Value.Update(gameTime);
                }
                foreach (int i in removeObjectsList)
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

        private static T addObject<T>(Serial serial) where T : Entity
        {
            T o = (T)Activator.CreateInstance(typeof(T), new object[] { serial, _world });
            // o.World = TileEngine.WorldStatic; // Add the world service (for movement).
            // If this object is the client, designate it to return events.
            if (o.Serial == MySerial)
                o.Movement.DesignateClientPlayer();
            _entities.Add(o.Serial, o); // Add the object to the objects collection.
            return (T)o;
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

        public static Entity GetPlayerObject()
        {
            // This could be cached to save time.
            if (_entities.ContainsKey(MySerial))
                return _entities[MySerial];
            else

                return null;
        }

        public static void Reset()
        {
            _entities.Clear();
        }
    }
}
