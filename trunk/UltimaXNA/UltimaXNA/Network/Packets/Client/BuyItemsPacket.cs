/***************************************************************************
 *   BuyItemsPacket.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

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
