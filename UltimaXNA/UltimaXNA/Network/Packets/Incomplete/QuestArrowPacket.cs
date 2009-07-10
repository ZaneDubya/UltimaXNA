using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class QuestArrowPacket : RecvPacket
    {
        public QuestArrowPacket(PacketReader reader)
            : base(0xBA, "Quest Arrow")
        {
            // TODO: Write this packet.
        }
    }
}
