#region File Description & Usings
//-----------------------------------------------------------------------------
// GameObjects.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System.Collections.Generic;
using Microsoft.Xna.Framework;
#endregion

namespace UndeadClient.GameObjects
{
    public interface IGameObjects
    {
        int MyGUID { get; set; }
        BaseObject AddObject(BaseObject nObject);
        BaseObject GetObject(int nGUID);
    }

    class GameObjects : GameComponent, IGameObjects
    {
        private Dictionary<int, BaseObject> m_Objects = new Dictionary<int, BaseObject>();

        public int MyGUID { get; set; }

        private TileEngine.IWorld m_WorldService;
        private IGameState m_GameStateService;

        public GameObjects(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IGameObjects), this);
        }

        public override void Initialize()
        {
            m_WorldService = (TileEngine.IWorld)Game.Services.GetService(typeof(TileEngine.IWorld));
            m_GameStateService = (IGameState)Game.Services.GetService(typeof(IGameState));
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (m_GameStateService.InWorld)
            {
                foreach (KeyValuePair<int, BaseObject> iObjectPair in m_Objects)
                {
                    if ((iObjectPair.Value.ObjectType & ObjectType.Unit) == ObjectType.Unit)
                    {
                        iObjectPair.Value.Update(gameTime);
                    }
                    if ((iObjectPair.Value.ObjectType & ObjectType.GameObject) == ObjectType.GameObject)
                    {
                        iObjectPair.Value.Update(gameTime);
                    }
                }
            }
            base.Update(gameTime);
        }

        public BaseObject AddObject(BaseObject nObject)
        {
            try
            {
                m_Objects.Add(nObject.GUID, nObject);
                nObject.World = m_WorldService;
                // If this object is the client, designate it to return events.
                if (nObject.GUID == MyGUID)
                    nObject.Movement.DesignateClientPlayer();
            }
            catch
            {
                // This object is already in the collection.
            }
            return GetObject(nObject.GUID);
        }

        public BaseObject GetObject(int nGUID)
        {
            // Check for existence here.
            if (m_Objects.ContainsKey(nGUID))
            {
                // Return the value.
                return m_Objects[nGUID];
            }

            // The key does not exist, return the default.
            return null;
        }
    }
}
