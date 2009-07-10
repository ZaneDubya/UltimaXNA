using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class TipNoticePacket : RecvPacket
    {
        public TipNoticePacket(PacketReader reader)
            : base(0xA6, "Tip Notice")
        {
            // TODO: Write this packet.
        }
    }
}
