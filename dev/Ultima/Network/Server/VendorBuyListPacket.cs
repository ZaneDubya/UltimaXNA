/***************************************************************************
 *   VendorBuyListPacket.cs
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
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.UltimaPackets.Server
{
    public class VendorBuyListPacket : RecvPacket
    {
        readonly Serial m_vendorPackSerial;
        readonly int m_itemCount;
        readonly int[] m_prices;
        readonly string[] m_descriptions;

        public Serial VendorPackSerial
        {
            get { return m_vendorPackSerial; }
        }

        public int ItemCount
        {
            get { return m_vendorPackSerial; }
        }

        public int[] Prices
        {
            get { return m_prices; } 
        }

        public string[] Descriptions
        {
            get { return m_descriptions; }
        }

        public VendorBuyListPacket(PacketReader reader)
            : base(0x74, "Open Buy Window")
        {
            m_vendorPackSerial = reader.ReadInt32();
            m_itemCount = reader.ReadByte();
            m_prices = new int[m_itemCount];
            m_descriptions = new string[m_itemCount];

            for (int i = 0; i < m_itemCount; i++)
            {
                m_prices[i] = reader.ReadInt32();
                int descriptionLegnth = reader.ReadByte();
                m_descriptions[i] = reader.ReadString(descriptionLegnth);
            }
        }
    }
}
