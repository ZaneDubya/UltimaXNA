using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class DeathAnimationPacket : RecvPacket
    {
        public DeathAnimationPacket(PacketReader reader)
            : base(0xAF, "Death Animation")
        {
            // TODO: Write this packet.
        }
    }
}
