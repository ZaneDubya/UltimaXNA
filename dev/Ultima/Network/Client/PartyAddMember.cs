using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client
{
    class PParty_AddMember : SendPacket
    {
        public PParty_AddMember() : base(0xbf, "Add Party Member")
        {
            Stream.Write((short)6);
            Stream.Write((byte)1);
            Stream.Write(0);
        }

    }
}
