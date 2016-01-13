using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client
{
    public class PQueryStats : SendPacket
    {
        public PQueryStats(int Serial) : base(0x34, "Query Stats", 10)
        {
            Stream.Write(-303174163);
            Stream.Write((byte)4);
            Stream.Write(Serial);
        }
    }
}
