using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client
{
    public class PParty_RemoveMember : SendPacket
    {
        public PParty_RemoveMember() : base(0xbf, "Remove Party Member")
        {
            Stream.Write((short)6);
            Stream.Write((byte)2);
            Stream.Write(0);
        }
    }
}
