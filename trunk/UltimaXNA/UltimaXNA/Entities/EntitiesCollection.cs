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
using Microsoft.Xna.Framework;
using UltimaXNA.Network.Packets.Client;
#endregion

namespace UltimaXNA.Entities
{
    public interface IEntitiesService
    {
        int MySerial { get; set; }
        T GetObject<T>(Serial serial, bool create) where T : Entity;
        Overhead AddOverhead(Serial serial);
        Entity GetPlayerObject();
        void RemoveObject(Serial serial);
        void Reset();
    }

    class EntitiesCollection : GameComponent, IEntitiesService
    {
        private Dictionary<int, Entity> _entities = new Dictionary<int, Entity>();
        public int MySerial { get; set; }
        private TileEngine.IWorld _worldService;
        private IGameState _gameStateService;
        private Client.IUltimaClient _gameClientService;
        private GUI.IGUI _GUIService;

        public EntitiesCollection(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IEntitiesService), this);
        }

        public override void Initialize()
        {
            _worldService = (TileEngine.IWorld)Game.Services.GetService(typeof(TileEngine.IWorld));
            _gameStateService = (IGameState)Game.Services.GetService(typeof(IGameState));
            _gameClientService = (Client.IUltimaClient)Game.Services.GetService(typeof(Client.IUltimaClient));
            _GUIService = (GUI.IGUI)Game.Services.GetService(typeof(GUI.IGUI));
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (_gameStateService.InWorld)
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
            base.Update(gameTime);
        }

        public Overhead AddOverhead(Serial serial)
        {
            Entity ownerEntity = _entities[serial];
            if (ownerEntity != null)
            {
                Overhead overhead = ownerEntity.AddOverhead();
                return overhead;
            }
            return null;
        }
        
        public T GetObject<T>(Serial serial, bool create) where T : Entity
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

        private T addObject<T>(Serial serial) where T : Entity
        {
            T o = (T)Activator.CreateInstance(typeof(T), new object[] { serial });
            o.World = _worldService; // Add the world service (for movement).
            // If this object is the client, designate it to return events.
            if (o.Serial == MySerial)
                o.Movement.DesignateClientPlayer();
            _entities.Add(o.Serial, o); // Add the object to the objects collection.
            return (T)o;
        }

        public void RemoveObject(Serial serial)
        {
            // When Dispose() is called, the object will tidy up and then
            // set m_Dispose = true. Reference this with IsDisposed on the
            // next update cycle.
            if (_entities.ContainsKey(serial))
            {
                _entities[serial].Dispose();
            }
        }

        public Entity GetPlayerObject()
        {
            // This could be cached to save time.
            if (_entities.ContainsKey(MySerial))
                return _entities[MySerial];
            else

                return null;
        }

        public void Reset()
        {
            _entities.Clear();
        }
    }
}
