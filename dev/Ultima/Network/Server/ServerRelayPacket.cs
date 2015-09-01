/***************************************************************************
 *   ServerRelayPacket.cs
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
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class ServerRelayPacket : RecvPacket
    {
        readonly int m_ipAddress;
        readonly int m_port;
        readonly int m_accountId;

        public int IpAddress
        {
            get { return m_ipAddress; }
        }

        public int Port
        {
            get { return m_port; }
        }

        public int AccountId
        {
            get { return m_accountId; }
        }

        public ServerRelayPacket(PacketReader reader)
            : base(0x8C, "Server Relay")
        {
            m_ipAddress = reader.ReadInt32();
            m_port = reader.ReadInt16();
            m_accountId = reader.ReadInt32();
        }
    }
}
