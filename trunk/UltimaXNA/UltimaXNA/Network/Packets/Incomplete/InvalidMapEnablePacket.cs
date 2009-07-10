using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class InvalidMapEnablePacket : RecvPacket
    {
        public InvalidMapEnablePacket(PacketReader reader)
            : base(0xC6, "Invalid Map Enable")
        {
            // TODO: Write this packet.
        }
    }
}
