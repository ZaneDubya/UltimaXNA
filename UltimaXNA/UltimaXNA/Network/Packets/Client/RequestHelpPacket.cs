using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class RequestHelpPacket : SendPacket
    {
        public RequestHelpPacket()
            : base(0x9B, "Request Help", 258)
        {
            byte[] empty = new byte[257];
            Stream.Write(empty, 0, 257);
        }
    }
}
