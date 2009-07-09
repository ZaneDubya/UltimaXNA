using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class GetPlayerStatusPacket : SendPacket
    {
        public GetPlayerStatusPacket(byte type, Serial serial)
            : base(0x34, "Get Player Status", 10)
        {
            Stream.Write((int)0);//Unknown
            Stream.Write((byte)type);
            Stream.Write(serial);
        }
    }
}
