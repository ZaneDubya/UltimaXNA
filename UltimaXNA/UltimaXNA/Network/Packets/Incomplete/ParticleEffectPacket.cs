using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class ParticleEffectPacket : RecvPacket
    {
        public ParticleEffectPacket(PacketReader reader)
            : base(0xC7, "Particle Effect")
        {
            // TODO: Write this packet.
        }
    }
}
