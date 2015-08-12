/***************************************************************************
 *   ContainerContentPacket.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
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
using UltimaXNA.Ultima.Data;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class ContainerContentPacket : RecvPacket
    {
        public static bool NextContainerContentsIsPre6017 = false;

        private ItemInContainer[] m_items;

        public ItemInContainer[] Items
        {
            get { return m_items; }
            set { m_items = value; }
        }

        public ContainerContentPacket(PacketReader reader)
            : base(0x3C, "Container ContentPacket")
        {
            int itemCount = reader.ReadUInt16();
            List<ItemInContainer> items = new List<ItemInContainer>(itemCount);

            for (int i = 0; i < itemCount; i++)
            {
                Serial serial = reader.ReadInt32();
                int iItemID = reader.ReadUInt16();
                int iUnknown = reader.ReadByte(); // signed, itemID offset. always 0 in RunUO.
                int iAmount = reader.ReadUInt16();
                int iX = reader.ReadInt16();
                int iY = reader.ReadInt16();
                int iGridLocation = 0;
                if (!NextContainerContentsIsPre6017)
                    iGridLocation = reader.ReadByte(); // always 0 in RunUO.
                int iContainerSerial = reader.ReadInt32();
                int iHue = reader.ReadUInt16();

                items.Add(new ItemInContainer(serial, iItemID, iAmount, iX, iY, iGridLocation, iContainerSerial, iHue));
            }

            m_items = items.ToArray();
            if (NextContainerContentsIsPre6017)
                NextContainerContentsIsPre6017 = false;
        }
    }
}
