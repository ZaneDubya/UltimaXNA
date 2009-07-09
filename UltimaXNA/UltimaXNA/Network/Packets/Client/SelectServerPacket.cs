using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class SelectServerPacket : SendPacket
    {
        public SelectServerPacket(int id) :
            base(0xA0, "Select Server", 3)
        {
            Stream.Write((short)id);
        }
    }
}
