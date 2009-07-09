using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class ResurrectionMenuPacket : RecvPacket
    {
        public ResurrectionMenuPacket(PacketReader reader)
            : base(0x2C, "Resurrection Menu")
        {

        }
    }
}
