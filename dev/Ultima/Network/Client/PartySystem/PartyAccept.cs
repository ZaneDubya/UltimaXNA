using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Network.Packets;
using UltimaXNA.Ultima.World.Entities.Mobiles;

namespace UltimaXNA.Ultima.Network.Client
{
    public class PartyAccept : SendPacket
    {
        public PartyAccept(Mobile Leader) : base(0xbf, "Party Join Accept")
        {
            Stream.Write((short)6);
            Stream.Write((byte)8);
            Stream.Write(Leader.Serial);
        }
    }
}
