using System;
using Microsoft.Xna.Framework;
using UltimaXNA.Network.Packets.Client;

namespace UltimaXNA.GUI
{
    public delegate void LoginEvent(string server, int port, string account, string password);

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

        public static void UseItem(GameObjects.Item item)
        {
            _GameClientService.Send(new DoubleClickPacket(item.Serial));
        }

        public static void PickupItem(GameObjects.Entity entity)
        {
            GameObjects.Item iObject = ((GameObjects.Item)entity);
            _GameClientService.Send(new PickupItemPacket(iObject.Serial, (short)iObject.Item_StackCount));
        }

        public static void DropItem(GameObjects.Entity entity, int x, int y, int z, Serial destEntity)
        {
            GameObjects.Item iObject = ((GameObjects.Item)entity);
            _GameClientService.Send(new DropItemPacket(iObject.Serial, (short)x, (short)y, (byte)z, 0, destEntity));
        }           
    }
}