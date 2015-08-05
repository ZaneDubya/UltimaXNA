/***************************************************************************
 *   VendorBuyListPacket.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
using System.Collections.Generic;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class VendorBuyListPacket : RecvPacket
    {
        public Serial VendorPackSerial
        {
            get;
            private set;
        }


        public List<VendorBuyItem> Items
        {
            get;
            private set;
        }

        public VendorBuyListPacket(PacketReader reader)
            : base(0x74, "Open Buy Window")
        {
            VendorPackSerial = reader.ReadInt32();
            int count = reader.ReadByte();
            Items = new List<VendorBuyItem>();
            for (int i = 0; i < count; i++)
            {
                int price = reader.ReadInt32();
                int descriptionLegnth = reader.ReadByte();
                string description = reader.ReadString(descriptionLegnth);

                Items.Add(new VendorBuyItem(price, description));
            }
        }

        public class VendorBuyItem
        {
            public readonly int Price;
            public readonly string Description;

            public VendorBuyItem(int price, string description)
            {
                Price = price;
                Description = description;
            }
        }
    }
}
