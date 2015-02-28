/***************************************************************************
 *   ServerListPacket.cs
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
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.UltimaNetwork.Packets.Server
{
    public class ServerListPacket : RecvPacket
    {
        readonly byte _flags;
        readonly ServerListEntry[] _servers;

        public byte Flags
        {
            get { return _flags; }
        }

        public ServerListEntry[] Servers
        {
            get { return _servers; }
        }

        public ServerListPacket(PacketReader reader)
            : base(0xA8, "Server List")
        {
            _flags = reader.ReadByte();
            ushort count = (ushort)reader.ReadInt16();

            _servers = new ServerListEntry[count];

            for (ushort i = 0; i < count; i++)
            {
                _servers[i] = new ServerListEntry(reader); ;
            }
        }
    }
}
