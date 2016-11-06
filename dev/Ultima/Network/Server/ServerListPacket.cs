/***************************************************************************
 *   ServerListPacket.cs
 *   Copyright (c) 2009 UltimaXNA Development Team
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
using UltimaXNA.Ultima.Login.Data;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class ServerListPacket : RecvPacket
    {
        readonly byte m_flags;
        readonly ServerListEntry[] m_servers;

        public byte Flags
        {
            get { return m_flags; }
        }

        public ServerListEntry[] Servers
        {
            get { return m_servers; }
        }

        public ServerListPacket(PacketReader reader)
            : base(0xA8, "Server List")
        {
            m_flags = reader.ReadByte();
            ushort count = (ushort)reader.ReadInt16();

            m_servers = new ServerListEntry[count];

            for (ushort i = 0; i < count; i++)
            {
                m_servers[i] = new ServerListEntry(reader);
            }
        }
    }
}
