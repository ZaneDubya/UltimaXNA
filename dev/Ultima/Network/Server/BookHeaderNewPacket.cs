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
        public readonly byte Flag0;
        public readonly byte Flag1;
        public readonly short Pages;
        public readonly short AuthorLength;
        public readonly string Author;
        public readonly short TitleLength;
        public readonly string Title;

        public BookHeaderNewPacket(PacketReader reader)
            : base(0xD4, "Book Header (New)")
        {
            Serial = reader.ReadInt32();
            Flag0 = reader.ReadByte();
            Flag1 = reader.ReadByte();
            Pages = reader.ReadInt16();
            TitleLength = reader.ReadInt16();
            Title = reader.ReadString();
            AuthorLength = reader.ReadInt16();
            Author = reader.ReadString();
        }
    }
}
