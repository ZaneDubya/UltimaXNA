/***************************************************************************
 *   BookHeaderNewPacket.cs
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
using UltimaXNA.Ultima.World.Entities.Items;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class BookHeaderNewPacket : RecvPacket
    {
        public readonly Serial Serial;
        public readonly byte flag1;
        public readonly byte flag2;
        public readonly short pages;
        public readonly short authorLength;
        public readonly string author;
        public readonly short titleLength;
        public readonly string title;

        public BookHeaderNewPacket(PacketReader reader)
            : base(0xD4, "Book Header (New)")
        {
            Serial = reader.ReadInt32();
            flag1 = reader.ReadByte();
            flag2 = reader.ReadByte();
            pages = reader.ReadInt16();

            titleLength = reader.ReadInt16();
            title = reader.ReadString();

            authorLength = reader.ReadInt16();
            author = reader.ReadString();
        }
    }
}
