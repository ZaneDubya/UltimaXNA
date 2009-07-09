using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class SingleClickPacket : SendPacket
    {
        public SingleClickPacket(Serial serial)
            : base(0x09, "Single Click", 5)
        {
            Stream.Write(serial);
        }
    }
}
