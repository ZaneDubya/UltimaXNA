using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Network.Packets;
using UltimaXNA.Ultima.World.Entities.Mobiles;

namespace UltimaXNA.Ultima.Network.Client
{
    public class PParty_Accept : SendPacket
    {
        public PParty_Accept(Mobile req) : base(0xbf, "Party Join Accept")
        {
            Stream.Write((short)6);
            Stream.Write((byte)8);
            Stream.Write(req.Serial);
        }
    }
}
