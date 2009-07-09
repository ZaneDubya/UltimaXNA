using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class RequestContextMenuPacket : SendPacket
    {
        public RequestContextMenuPacket(Serial serial)
            : base(0xBF, "Context Menu Request")
        {
            Stream.Write((short)0x13);
            Stream.Write(serial);
        }
    }
}
