using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class PickupItemPacket : SendPacket
    {
        public PickupItemPacket(Serial serial, short count)
            : base(0x07, "Pickup Item", 7)
        {
            Stream.Write(serial);
            Stream.Write((short)count);
        }
    }
}
