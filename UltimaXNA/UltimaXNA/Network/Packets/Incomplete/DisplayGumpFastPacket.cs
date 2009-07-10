using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class DisplayGumpFastPacket : RecvPacket
    {
        public DisplayGumpFastPacket(PacketReader reader)
            : base(0xB0, "Display Gump Fast")
        {
            // TODO: Write this packet.
        }
    }
}
