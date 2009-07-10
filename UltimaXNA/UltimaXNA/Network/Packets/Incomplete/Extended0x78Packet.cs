using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class Extended0x78Packet : RecvPacket
    {
        public Extended0x78Packet(PacketReader reader)
            : base(0xD3, "Extended 0x78")
        {
            // TODO: Write this packet.
        }
    }
}
