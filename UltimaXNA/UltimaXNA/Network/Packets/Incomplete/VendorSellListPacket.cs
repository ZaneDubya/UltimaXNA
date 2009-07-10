using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class VendorSellListPacket : RecvPacket
    {
        public VendorSellListPacket(PacketReader reader)
            : base(0x9E, "Vendor Sell List")
        {
            // TODO: Write this packet.
        }
    }
}
