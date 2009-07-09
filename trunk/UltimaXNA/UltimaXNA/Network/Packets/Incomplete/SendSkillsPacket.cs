using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class SendSkillsPacket : RecvPacket
    {
        public SendSkillsPacket(PacketReader reader)
            : base(0x3A, "Send Skills List")
        {

        }
    }
}
