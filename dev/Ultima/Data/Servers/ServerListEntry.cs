using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Network;

namespace UltimaXNA.Ultima.Data.Servers
{
    public class ServerListEntry
    {
        readonly ushort index;
        readonly string name;
        readonly byte percentFull;
        readonly byte timezone;
        readonly uint address;

        public ushort Index
        {
            get { return index; }
        }

        public string Name
        {
            get { return name; }
        }

        public byte PercentFull
        {
            get { return percentFull; }
        }

        public byte Timezone
        {
            get { return timezone; }
        }

        public uint Address
        {
            get { return address; }
        }

        public ServerListEntry(PacketReader reader)
        {
            index = (ushort)reader.ReadInt16();
            name = reader.ReadString(30);
            percentFull = reader.ReadByte();
            timezone = reader.ReadByte();
            address = (uint)reader.ReadInt32();
        }
    }
}
