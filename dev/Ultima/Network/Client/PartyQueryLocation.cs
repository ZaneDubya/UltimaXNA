using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client
{
    public class PPE_QueryPartyLocs : SendPacket
    {
        public PPE_QueryPartyLocs() : base(240, "Query Party Locations")
        {
            Stream.Write((byte)0);
        }
    }
}
