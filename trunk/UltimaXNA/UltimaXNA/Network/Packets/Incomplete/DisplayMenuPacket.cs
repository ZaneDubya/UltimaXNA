using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class DisplayMenuPacket : RecvPacket
    {
        public DisplayMenuPacket(PacketReader reader)
            : base(0x7C, "Display Menu")
        {
            // TODO: Write this packet.
        }
    }
}
