using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class TimePacket : RecvPacket
    {
        public TimePacket(PacketReader reader)
            : base(0x5B, "Time")
        {

        }
    }
}
