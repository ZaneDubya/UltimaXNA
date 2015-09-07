/***************************************************************************
 *   CorpseClothingPacket.cs
 *   Copyright (c) 2009 UltimaXNA Development Team
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class CorpseClothingPacket : RecvPacket
    {
        public readonly Serial CorpseSerial;
        public readonly List<CorpseItem> Items = new List<CorpseItem>();
        public CorpseClothingPacket(PacketReader reader)
            : base(0x89, "Corpse Clothing")
        {
            CorpseSerial = reader.ReadInt32(); // BYTE[4] corpseID
            bool hasItems = true;
            while (hasItems)
            {
                int layer = reader.ReadByte();
                if (layer == 0x00)
                {
                    hasItems = false;
                }
                else
                {
                    Serial itemSerial = reader.ReadInt32();
                    Items.Add(new CorpseItem(layer, itemSerial));
                }
            }
        }

        public struct CorpseItem
        {
            public int Layer;
            public Serial Serial;

            public CorpseItem(int layer, Serial serial)
            {
                Layer = layer;
                Serial = serial;
            }
        }
    }
}
