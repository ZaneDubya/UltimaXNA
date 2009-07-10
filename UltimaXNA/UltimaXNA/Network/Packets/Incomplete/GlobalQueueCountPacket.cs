using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class GlobalQueuePacket : RecvPacket
    {
        public GlobalQueuePacket(PacketReader reader)
            : base(0xCB, "Global Queue")
        {
            // TODO: Write this packet.
        }
    }
}
