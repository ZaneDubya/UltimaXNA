using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Network.Packets;
using UltimaXNA.Ultima.World.Entities.Mobiles;

namespace UltimaXNA.Ultima.Network.Client
{
    public class PartyDecline : SendPacket
    {
        public PartyDecline(Mobile Leader) : base(0xbf, "Party Join Decline")
        {
            Stream.Write((short)6);
            Stream.Write((byte)9);
            Stream.Write(Leader.Serial);
        }
    }
}
