using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class HuedEffectPacket : RecvPacket
    {
        public HuedEffectPacket(PacketReader reader)
            : base(0xC0, "Hued Effect")
        {
            // TODO: Write this packet.
        }
    }
}
