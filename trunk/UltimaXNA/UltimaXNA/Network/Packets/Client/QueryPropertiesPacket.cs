using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class QueryPropertiesPacket : SendPacket
    {
        public QueryPropertiesPacket(Serial serial)
            : base(0xD6, "Query Properties", 7)
        {
            Stream.Write((short)7);
            Stream.Write((int)serial);
        }
    }
}
