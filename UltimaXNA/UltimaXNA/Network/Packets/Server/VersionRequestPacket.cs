using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class VersionRequestPacket : RecvPacket
    {
        public VersionRequestPacket(PacketReader reader)
            : base(0xBD, "Client Version Request")
        {
            // no data.
        }
    }
}
