using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class GraphicEffectPacket : RecvPacket
    {
        public GraphicEffectPacket(PacketReader reader)
            : base(0x70, "Show Graphic Effect")
        {

        }
    }
}
