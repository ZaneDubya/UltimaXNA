/***************************************************************************
 *   BookPageChangePacket.cs
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
    public class BookPageChangePacket : SendPacket
    {
        public BookPageChangePacket(Serial serial, int page, string[] lines)
            : base(0x66, "Book Page Change")
        {
            Stream.Write(serial);
            Stream.Write((short)1); // Page count always 1
            Stream.Write((short)(page + 1)); // Page number
            Stream.Write((short)lines.Length); // Number of lines
            // Send each line of the page
            for (int i = 0; i < lines.Length; i++)
            {
                Stream.WriteUTF8Null(lines[i]);
            }
        }
    }
}
