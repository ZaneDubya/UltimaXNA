using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Network.Packets;
using UltimaXNA.Ultima.World.Entities.Mobiles;

namespace UltimaXNA.Ultima.Network.Client
{
    public class PParty_Decline : SendPacket
    {
        public PParty_Decline(Mobile req) : base(0xbf, "Party Join Decline")
        {
            Stream.Write((short)6);
            Stream.Write((byte)9);
            Stream.Write(req.Serial);
        }
    }
}
