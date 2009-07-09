using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class DoubleClickPacket : SendPacket
    {
        public DoubleClickPacket(Serial serial)
            : base(0x06, "Double Click", 5)
        {
            Stream.Write(serial);
        }
    }
}
