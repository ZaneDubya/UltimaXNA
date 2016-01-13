using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client
{
    public class PParty_SetCanLoot : SendPacket
    {
        public PParty_SetCanLoot(bool val) : base(0xbf, "Party Set Can Loot")
        {
           Stream.Write((short)6);
            Stream.Write((byte)6);
            Stream.Write(val);
        }
    }
}
