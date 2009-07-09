using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class SellListReplyPacket : SendPacket
    {
        public SellListReplyPacket(Serial vendorSerial, Pair<int, short>[] items)
            : base(0x9F, "Sell List Reply")
        {
            Stream.Write(vendorSerial);
            Stream.Write((short)items.Length);

            for (int i = 0; i < items.Length; i++)
            {
                Stream.Write(items[i].ItemA);
                Stream.Write((short)items[i].ItemB);
            }
        }
    }
}
