using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Network.Packets;
using UltimaXNA.Ultima.World.Entities.Mobiles;

namespace UltimaXNA.Ultima.Network.Client
{
    public class PParty_PrivateMessage : SendPacket
    {
        public PParty_PrivateMessage(Mobile who, string text) : base(0xbf, "Private Party Message")
        {
            Stream.Write((short)6);
            Stream.Write((byte)3);
            Stream.Write(who.Serial);
            Stream.WriteBigUniNull(text);
            Stream.Write((short)0);
        }
    }
}
