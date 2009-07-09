using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class OpenPaperdollPacket : RecvPacket
    {
        public OpenPaperdollPacket(PacketReader reader)
            : base(0x88, "Open Paperdoll")
        {

        }
    }
}
