#region File Description & Usings
//-----------------------------------------------------------------------------
// GameObjects.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System.Collections.Generic;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.GameObjects
{
    public interface IGameObjects
    {
        int MyGUID { get; set; }
        BaseObject AddObject(BaseObject nObject);
        BaseObject GetObject(int nGUID);
        BaseObject GetContainerObject(int nGUID);
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
                        case ObjectType.Container:
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

        public BaseObject AddObject(BaseObject nObject)
        {
            try
            {
                m_Objects.Add(nObject.GUID, nObject);
                nObject.World = m_WorldService;
                // If this object is the client, designate it to return events.
                if (nObject.GUID == MyGUID)
                    nObject.Movement.DesignateClientPlayer();

                if ((nObject.ObjectType & ObjectType.Unit) == ObjectType.Unit)
                {
                    ((Unit)nObject).UpdateHealthStaminaMana += this.Unit_UpdateHealthStaminaMana;
                }
                if ((nObject.ObjectType & ObjectType.GameObject) == ObjectType.GameObject)
                {
                    ((GameObject)nObject).SendPacket_MoveItemWithinContainer += this.Item_Packet_MoveItemWithinContainer;
                }
            }
            catch
            {
                // This object is already in the collection.
            }
            return GetObject(nObject.GUID);
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

        public BaseObject GetPlayerObject()
        {
            return m_Objects[MyGUID];
        }

        public BaseObject GetContainerObject(int nGUID)
        {
            // Check for existence here.
            if (m_Objects.ContainsKey(nGUID))
            {
                // We know that m_Objects has an object with this GUID. Now we determine if the object
                // can expose a container object.
                switch (m_Objects[nGUID].ObjectType)
                {
                    case ObjectType.Container:
                        return m_Objects[nGUID];
                    case ObjectType.GameObject:
                        return ((GameObject)m_Objects[nGUID]).ContainerObject;
                    default:
                        return null;
                }
            }
            // The key does not exist, return the default.
            return null;
        }

        private void Item_Packet_MoveItemWithinContainer(BaseObject nObject)
        {
            GameObject iObject = ((GameObject)nObject);
            m_GameClientService.Send_PickUpItem(iObject.GUID, iObject.Item_StackCount);
            m_GameClientService.Send_DropItem(iObject.GUID,
                iObject.Item_InvX_SlotIndex, iObject.Item_InvY_SlotChecksum,
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
