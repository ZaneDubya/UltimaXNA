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
        private static IGameState _GameStateService;
        private static Client.IUltimaClient _GameClientService;

        public static void Initialize(GameServiceContainer container)
        {
            _GameStateService = (IGameState)container.GetService(typeof(IGameState));
            _GameClientService = (Client.IUltimaClient)container.GetService(typeof(Client.IUltimaClient));
        }

        public static void QuitImmediate()
        {
            _GameStateService.EngineRunning = false;
        }

        public static bool Connect(string host, int port)
        {
            return _GameClientService.Connect(host, port);
        }

        public static void Login(string username, string password)
        {
            _GameClientService.SetAccountPassword(username, password);
            _GameClientService.Send(new LoginPacket(username, password));
        }

        public static void PickupItem(GameObjects.BaseObject entity)
        {
            GameObjects.GameObject iObject = ((GameObjects.GameObject)entity);
            _GameClientService.Send(new PickupItemPacket(iObject.Serial, (short)iObject.Item_StackCount));
        }

        public static void DropItem(GameObjects.BaseObject entity, int x, int y, int z, Serial destEntity)
        {
            GameObjects.GameObject iObject = ((GameObjects.GameObject)entity);
            _GameClientService.Send(new DropItemPacket(iObject.Serial, (short)x, (short)y, (byte)z, 0, destEntity));
        }           
    }
}