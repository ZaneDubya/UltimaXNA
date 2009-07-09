using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
