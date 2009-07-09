using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class CompressedGumpPacket : RecvPacket
    {
        public CompressedGumpPacket(PacketReader reader)
            : base(0xDD, "Compressed Gump")
        {

        }
    }
}
