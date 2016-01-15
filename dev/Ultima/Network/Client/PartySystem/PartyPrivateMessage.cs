using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client
{
    public class PartyPrivateMessage : SendPacket
    {
        public PartyPrivateMessage(Serial memberSerial, string PMsg) : base(0xbf, "Private Party Message")
        {
            Stream.Write((short)6);
            Stream.Write((byte)3);
            Stream.Write(memberSerial);
            Stream.WriteBigUniNull(PMsg);
            Stream.Write((short)0);
        }
    }
}
