using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client
{
    public class PartyCanLoot : SendPacket
    {
        public PartyCanLoot(bool isLootable) : base(0xbf, "Party Can Loot")
        {
           Stream.Write((short)6);
            Stream.Write((byte)6);
            Stream.Write(isLootable);
        }
    }
}
