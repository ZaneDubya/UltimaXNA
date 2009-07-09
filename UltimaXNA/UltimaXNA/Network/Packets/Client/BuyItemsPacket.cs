using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class BuyItemsPacket : SendPacket
    {
        public BuyItemsPacket(Serial vendorSerial, Pair<int, short>[] items)
            : base(0x3B, "Buy Items")
        {
            Stream.Write(vendorSerial);
            Stream.Write((byte)0x02); // flag

            for (int i = 0; i < items.Length; i++)
            {
                Stream.Write((byte)0x1A); // layer?
                Stream.Write(items[i].ItemA);
                Stream.Write((short)items[i].ItemB);
            }
        }
    }
}
