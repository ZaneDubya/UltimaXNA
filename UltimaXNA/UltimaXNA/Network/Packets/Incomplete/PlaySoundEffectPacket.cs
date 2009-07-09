using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class PlaySoundEffectPacket : RecvPacket
    {
        public PlaySoundEffectPacket(PacketReader reader)
            : base(0x54, "Play Sound Effect")
        {

        }
    }
}
