using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class CorpseClothingPacket : RecvPacket
    {
        public readonly Serial CorpseSerial;
        public readonly List<CorpseClothingItemWithLayer> Items = new List<CorpseClothingItemWithLayer>();
        public CorpseClothingPacket(PacketReader reader)
            : base(0x89, "Corpse Clothing")
        {
            CorpseSerial = reader.ReadInt32(); // BYTE[4] corpseID
            bool isNotTerminated = false;
            while (isNotTerminated)
            {
                int layer = reader.ReadByte();
                if (layer == 0x00)
                {
                    isNotTerminated = false;
                }
                else
                {
                    Serial itemSerial = reader.ReadInt32();
                    Items.Add(new CorpseClothingItemWithLayer(layer, itemSerial));
                }
            }
        }
    }

    public struct CorpseClothingItemWithLayer
    {
        public int Layer;
        public Serial Serial;

        public CorpseClothingItemWithLayer(int layer, Serial serial)
        {
            Layer = layer;
            Serial = serial;
        }
    }
}
