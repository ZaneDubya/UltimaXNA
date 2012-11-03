/***************************************************************************
 *   VendorBuyListPacket.cs
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
using UltimaXNA.Network;
#endregion

namespace UltimaXNA.Network.Packets.Server
{
    public class VendorBuyListPacket : RecvPacket
    {
        readonly Serial _vendorPackSerial;
        readonly int _itemCount;
        readonly int[] _prices;
        readonly string[] _descriptions;

        public Serial VendorPackSerial
        {
            get { return _vendorPackSerial; }
        }

        public int ItemCount
        {
            get { return _vendorPackSerial; }
        }

        public int[] Prices
        {
            get { return _prices; } 
        }

        public string[] Descriptions
        {
            get { return _descriptions; }
        }

        public VendorBuyListPacket(PacketReader reader)
            : base(0x74, "Open Buy Window")
        {
            _vendorPackSerial = reader.ReadInt32();
            _itemCount = reader.ReadByte();
            _prices = new int[_itemCount];
            _descriptions = new string[_itemCount];

            for (int i = 0; i < this._itemCount; i++)
            {
                _prices[i] = reader.ReadInt32();
                int descriptionLegnth = reader.ReadByte();
                _descriptions[i] = reader.ReadString(descriptionLegnth);
            }
        }
    }
}
