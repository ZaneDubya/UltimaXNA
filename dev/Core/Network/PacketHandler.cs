/***************************************************************************
 *   PacketHandler.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;

namespace UltimaXNA.Core.Network {
    public abstract class PacketHandler {
        public readonly int ID;
        public readonly int Length;
        public readonly Type PacketType;
        public readonly object Client;

        public PacketHandler(int id, int length, Type packetType, object client) {
            ID = id;
            Length = length;
            PacketType = packetType;
            Client = client;
        }

        public abstract void Invoke(PacketReader reader);
    }

    public class PacketHandler<T> : PacketHandler where T : IRecvPacket {
        Action<T> m_Handler;

        public PacketHandler(int id, int length, Type packetType, object client, Action<T> handler)
            : base(id, length, packetType, client) {
            m_Handler = handler;
        }
        
        public override void Invoke(PacketReader reader) {
            T packet = (T)Activator.CreateInstance(PacketType, new object[] { reader });
            m_Handler(packet);
        }
    }
}
