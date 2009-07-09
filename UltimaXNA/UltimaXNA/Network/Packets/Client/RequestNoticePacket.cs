using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class RequestNoticePacket : SendPacket
    {
        public RequestNoticePacket(short lastNoticeNumber)
            : base(0xA7, "Request Notice", 4)
        {
            Stream.Write((short)lastNoticeNumber);
            Stream.Write((byte)0x01);
        }
    }
}
