using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
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
