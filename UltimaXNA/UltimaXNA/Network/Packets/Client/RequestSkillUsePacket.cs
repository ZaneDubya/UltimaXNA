using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class RequestSkillUsePacket : SendPacket
    {
        public RequestSkillUsePacket(int id)
            : base(0x12, "Request Skill Use")
        {
            Stream.Write((byte)0x24);
            Stream.WriteAsciiNull(String.Format("{0} 0", id));
        }
    }
}
