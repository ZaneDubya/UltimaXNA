using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class OpenWebBrowserPacket : RecvPacket
    {
        public OpenWebBrowserPacket(PacketReader reader)
            : base(0xA5, "Open Web Browser")
        {
            // TODO: Write this packet.
        }
    }
}
