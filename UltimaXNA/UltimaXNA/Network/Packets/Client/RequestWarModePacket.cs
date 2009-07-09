using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class RequestWarModePacket : SendPacket
    {
        public RequestWarModePacket(bool warMode)
            : base(0x72, "Request War Mode", 5)
        {
            Stream.Write(warMode);
            Stream.Write((byte)0x00);
            Stream.Write((byte)0x32);
            Stream.Write((byte)0x00);
        }
    }
}
