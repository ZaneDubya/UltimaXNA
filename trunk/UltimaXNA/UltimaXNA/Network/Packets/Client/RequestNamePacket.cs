using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class RequestNamePacket : SendPacket
    {
        public RequestNamePacket(Serial serial)
            : base(0x98, "Request Name", 7)
        {
            Stream.Write((short)7);
            Stream.Write((int)serial);
        }
    }
}
