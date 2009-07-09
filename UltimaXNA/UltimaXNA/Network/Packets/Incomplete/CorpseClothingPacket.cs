using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class CorpseClothingPacket : RecvPacket
    {
        public CorpseClothingPacket(PacketReader reader)
            : base(0x89, "Corpse Clothing")
        {

        }
    }
}
