using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class AttackRequestPacket : SendPacket
    {
        public AttackRequestPacket(Serial serial)
            : base(0x05, "Attack Request", 5)
        {
            Stream.Write(serial);
        }
    }
}
