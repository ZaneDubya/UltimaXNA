/***************************************************************************
 *   SellItemsPacket.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.Ultima.Network.Client
{
    public class SellItemsPacket : SendPacket
    {
        public SellItemsPacket(Serial vendorSerial, Tuple<int, short>[] items)
            : base(0x9F, "Sell Items")
        {
            Stream.Write(vendorSerial);
            Stream.Write((short)items.Length);

            for (int i = 0; i < items.Length; i++)
            {
                Stream.Write(items[i].Item1);
                Stream.Write((short)items[i].Item2);
            }
        }
    }
}
