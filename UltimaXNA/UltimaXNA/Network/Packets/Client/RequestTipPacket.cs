using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class RequestTipPacket : SendPacket
    {
        public RequestTipPacket(short lastTipNumber)
            : base(0xA7, "Request Tip", 4)
        {
            Stream.Write((short)lastTipNumber);
            Stream.Write((byte)0x00);
        }
    }
}
