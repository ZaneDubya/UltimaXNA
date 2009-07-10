using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class ContainerContentPacket : RecvPacket
    {
        private ContentItem[] _items;

        public ContentItem[] Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public ContainerContentPacket(PacketReader reader)
            : base(0x3C, "Container ContentPacket")
        {
            int itemCount = reader.ReadUInt16();
            List<ContentItem> items = new List<ContentItem>(itemCount);

            for (int i = 0; i < itemCount; i++)
            {
                Serial serial = reader.ReadInt32();
                int iItemID = reader.ReadUInt16();
                int iUnknown = reader.ReadByte(); // signed, itemID offset. always 0 in RunUO.
                int iAmount = reader.ReadUInt16();
                int iX = reader.ReadInt16();
                int iY = reader.ReadInt16();
                int iGridLocation = reader.ReadByte(); // always 0 in RunUO.
                int iContainerSerial = reader.ReadInt32();
                int iHue = reader.ReadUInt16();

                items.Add(new ContentItem(serial, iItemID, iAmount, iX, iY, iGridLocation, iContainerSerial, iHue));
            }

            _items = items.ToArray();
        }
    }
}
