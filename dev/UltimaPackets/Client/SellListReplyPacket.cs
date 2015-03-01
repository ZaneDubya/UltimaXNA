/***************************************************************************
 *   SellListReplyPacket.cs
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
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.UltimaPackets.Client
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
