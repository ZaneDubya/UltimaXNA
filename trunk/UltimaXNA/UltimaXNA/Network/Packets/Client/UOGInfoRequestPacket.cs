using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class UOGInfoRequestPacket : SendPacket
    {
        public UOGInfoRequestPacket()
            : base(0xF1, "UOG Information Request", 4)
        {
            Stream.Write((short)4);
            Stream.Write((byte)0xFF);
        }
    }
}
