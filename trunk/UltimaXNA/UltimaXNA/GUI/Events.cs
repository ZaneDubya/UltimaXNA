/***************************************************************************
 *   Events.cs
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
using Microsoft.Xna.Framework;
using UltimaXNA.Network.Packets.Client;
using UltimaXNA.Entities;
#endregion

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

        public static void UseItem(Item item)
        {
            _GameClientService.Send(new DoubleClickPacket(item.Serial));
        }

        public static void PickupItem(Entity entity)
        {
            Item iObject = ((Item)entity);
            _GameClientService.Send(new PickupItemPacket(iObject.Serial, (short)iObject.Amount));
        }

        public static void DropItem(Entities.Entity entity, int x, int y, int z, Serial destEntity)
        {
            Item iObject = ((Item)entity);
            _GameClientService.Send(new DropItemPacket(iObject.Serial, (short)x, (short)y, (byte)z, 0, destEntity));
        }           
    }
}