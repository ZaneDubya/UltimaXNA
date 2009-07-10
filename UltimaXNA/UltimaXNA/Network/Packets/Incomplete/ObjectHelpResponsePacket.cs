using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class ObjectHelpResponsePacket : RecvPacket
    {
        public ObjectHelpResponsePacket(PacketReader reader)
            : base(0xB7, "Display Menu")
        {
            // TODO: Write this packet.
        }
    }
}
