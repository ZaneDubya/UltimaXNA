/***************************************************************************
 *   CorpseClothingPacket.cs
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

namespace UltimaXNA.Client.Packets.Server
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
