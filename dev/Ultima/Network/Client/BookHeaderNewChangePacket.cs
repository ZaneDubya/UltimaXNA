/***************************************************************************
 *   BookHeaderNewChangePacket.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.Ultima.Network.Client
{
    public class BookHeaderNewChangePacket : SendPacket
    {
        public BookHeaderNewChangePacket(Serial serial, string title, string author)
            : base(0xD4, "Book Header Change (New)")
        {
            Stream.Write(serial);
            Stream.Write((byte)0); // Flag 1 = 0 
            Stream.Write((byte)0); // Flag 2 = 0
            Stream.Write((short)0); // Number of pages = 0

            Stream.Write((short)title.Length);
            Stream.WriteUTF8Fixed(title, title.Length);

            Stream.Write((short)author.Length);
            Stream.WriteUTF8Fixed(author, author.Length);
        }
    }
}
