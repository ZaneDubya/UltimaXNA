#region File Description & Usings
//-----------------------------------------------------------------------------
// GameObjects.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.GameObjects
{
    public interface IGameObjects
    {
        int MyGUID { get; set; }
        BaseObject GetObject(int nGUID, ObjectType nObjectType);
        BaseObject GetPlayerObject();
        void RemoveObject(int nGUID);
    }

    class GameObjects : GameComponent, IGameObjects
    {
        private Dictionary<int, BaseObject> m_Objects = new Dictionary<int, BaseObject>();

        public int MyGUID { get; set; }

        private TileEngine.IWorld m_WorldService;
        private IGameState m_GameStateService;
        private Network.IGameClient m_GameClientService;
        GUI.IGUI m_GUIService;

        public GameObjects(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IGameObjects), this);
        }

        public override void Initialize()
        {
            m_WorldService = (TileEngine.IWorld)Game.Services.GetService(typeof(TileEngine.IWorld));
            m_GameStateService = (IGameState)Game.Services.GetService(typeof(IGameState));
            m_GameClientService = (Network.IGameClient)Game.Services.GetService(typeof(Network.IGameClient));
            m_GUIService = (GUI.IGUI)Game.Services.GetService(typeof(GUI.IGUI));
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // We only need to update objects if we are in the world.
            if (m_GameStateService.InWorld)
            {
                List<int> iRemoveObjects = new List<int>();
                foreach (KeyValuePair<int, BaseObject> iObjectPair in m_Objects)
                {
                    // First check if we need to remove any objects. Objects that are due to be disposed
                    // are not updated, but are added to a list to be removed after we enumerate m_Objects.
                    if (iObjectPair.Value.IsDisposed)
                    {
                        iRemoveObjects.Add(iObjectPair.Key);
                        continue;
                    }

                    // Some object types need to be updated. Others do not.
                    switch (iObjectPair.Value.ObjectType)
                    {
                        case ObjectType.GameObject:
                        case ObjectType.Unit:
                        case ObjectType.Player:
                        {
                            iObjectPair.Value.Update(gameTime);
                            break;
                        }
                        default:
                        {
                            // no need to update.
                            break;
                        }
                    }
                }

                // Run through the list of objects needing to be removed from the collection.
                foreach (int i in iRemoveObjects)
                {
                    m_Objects.Remove(i);
                }         
            }
            base.Update(gameTime);
        }
        
        public BaseObject GetObject(int nGUID, ObjectType nObjectType)
        {
            // Check for existence in the collection.
            if (m_Objects.ContainsKey(nGUID))
            {
                // This object is in the m_Objects collection. If it is being disposed, then we should complete disposal
                // of the object and then return a new object. If it is not being disposed, return the object in the collection.
                if (m_Objects[nGUID].IsDisposed)
                {
                    m_Objects.Remove(nGUID);
                    return m_AddObject(nGUID, nObjectType);
                }
                return m_Objects[nGUID];
            }

            // No object with this GUID is in the collection. So we create a new one and return that, and hope that the server
            // will fill us in on the details of this object soon.
            return m_AddObject(nGUID, nObjectType);
        }

        private BaseObject m_AddObject(int nGUID, ObjectType nObjectType)
        {
            BaseObject iReturnObject;
            switch (nObjectType)
            {
                case ObjectType.Object:
                    iReturnObject = new BaseObject(nGUID);
                    break;
                case ObjectType.GameObject:
                    iReturnObject = new GameObject(nGUID);
                    break;
                case ObjectType.Unit:
                    iReturnObject = new Unit(nGUID);
                    break;
                case ObjectType.Player:
                    iReturnObject = new Player(nGUID);
                    break;
                default:
                    throw new Exception("Unhandled ObjectType in m_AddObject: " + nObjectType.ToString());
            }
            // Add the world service (for movement).
            iReturnObject.World = m_WorldService;
            // If this object is the client, designate it to return events.
            if (iReturnObject.GUID == MyGUID)
                iReturnObject.Movement.DesignateClientPlayer();
            // Add update events.
            if ((iReturnObject.ObjectType & ObjectType.Unit) == ObjectType.Unit)
            {
                ((Unit)iReturnObject).UpdateHealthStaminaMana += this.Unit_UpdateHealthStaminaMana;
            }
            if ((iReturnObject.ObjectType & ObjectType.GameObject) == ObjectType.GameObject)
            {
                ((GameObject)iReturnObject).SendPacket_MoveItemWithinContainer += this.Item_Packet_MoveItemWithinContainer;
            }
            // Add the object to the objects collection.
            m_Objects.Add(iReturnObject.GUID, iReturnObject);
            // Finally return the new object.
            return iReturnObject;
        }

        public void RemoveObject(int nGUID)
        {
            if (m_Objects.ContainsKey(nGUID))
            {
                m_Objects[nGUID].Dispose();
                // When Dispose() is called, the object will tidy up and then
                // set m_Dispose = true. Reference this with IsDisposed on the
                // next update cycle.
            }
        }

        public BaseObject GetPlayerObject()
        {
            // This could be cached to save time.
            return m_Objects[MyGUID];
        }

        private void Item_Packet_MoveItemWithinContainer(BaseObject nObject)
        {
            GameObject iObject = ((GameObject)nObject);
            m_GameClientService.Send_PickUpItem(iObject.GUID, iObject.Item_StackCount);
            m_GameClientService.Send_DropItem(iObject.GUID,
                iObject.Item_InvX, iObject.Item_InvY,
                0, iObject.Item_ContainedWithinGUID);
        }

        private void Unit_UpdateHealthStaminaMana(BaseObject nObject)
        {
            Unit iObject = ((Unit)nObject);
            if (iObject.GUID == MyGUID)
            {
                GUI.Window_StatusFrame w = (GUI.Window_StatusFrame)m_GUIService.Window("StatusFrame");
                w.CurrentHealth = iObject.Health.Current;
                w.MaxHealth = iObject.Health.Max;
                w.CurrentMana = iObject.Mana.Current;
                w.MaxMana= iObject.Mana.Max;
                w.CurrentStamina = iObject.Stamina.Current;
                w.MaxStamina = iObject.Stamina.Max;
            }
        }
    }
}
