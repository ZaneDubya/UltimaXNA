/***************************************************************************
 *   BookPagesPacket.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
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
using UltimaXNA.Ultima.World.Entities.Items;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class BookPagesPacket : RecvPacket
    {
        public readonly Serial Serial;
        public readonly int PageCount;
        public readonly BaseBook.BookPageInfo[] Pages;

        public BookPagesPacket(PacketReader reader)
            : base(0x66, "Book Pages")
        {
            Serial = reader.ReadInt32();
            PageCount = reader.ReadInt16();
            Pages = new BaseBook.BookPageInfo[PageCount];
            for (int i = 0; i < PageCount; ++i)
            {
                int page = reader.ReadInt16();
                int length = reader.ReadInt16();
                string[] lines = new string[length];
                for (int j = 0; j < length; j++)
                {
                    lines[j] = reader.ReadString();
                }
                Pages[i] = new BaseBook.BookPageInfo(lines);
            }
        }
    }
}
