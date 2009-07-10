using System;
using Microsoft.Xna.Framework;
using UltimaXNA.Network.Packets.Client;

namespace UltimaXNA.GUI
{

    // No scripted window should ever directly interact with the client.
    // All events should be handled through this class, which will provide
    // a simple interface for all interaction with the client.
    public static class Events
    {
        private static IGameState m_GameStateService;
        private static Client.IUltimaClient m_GameClientService;

        public static void Initialize(GameServiceContainer nContainer)
        {
            m_GameStateService = (IGameState)nContainer.GetService(typeof(IGameState));
            m_GameClientService = (Client.IUltimaClient)nContainer.GetService(typeof(Client.IUltimaClient));
        }

        public static void QuitImmediate()
        {
            m_GameStateService.EngineRunning = false;
        }

        public static bool Connect(string nHost, int nPort)
        {
            return m_GameClientService.Connect(nHost, nPort);
        }

        public static void Login(string nUsername, string nPassword)
        {
            m_GameClientService.SetAccountPassword(nUsername, nPassword);
            m_GameClientService.Send(new LoginPacket(nUsername, nPassword));
        }

        public static void PickupItem(GameObjects.BaseObject nObject)
        {
            GameObjects.GameObject iObject = ((GameObjects.GameObject)nObject);
            m_GameClientService.Send(new PickupItemPacket(iObject.GUID, (short)iObject.Item_StackCount));
        }

        public static void DropItem(GameObjects.BaseObject nObject, int x, int y, int z, Serial dest)
        {
            GameObjects.GameObject iObject = ((GameObjects.GameObject)nObject);
            m_GameClientService.Send(new DropItemPacket(iObject.GUID, (short)x, (short)y, (byte)z, 0, dest));
        }


                   
    }
}