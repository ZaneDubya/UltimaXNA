/***************************************************************************
 *   BookHeaderOldPacket.cs
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
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class BookHeaderOldPacket : RecvPacket
    {
        /*
             Packet Name: Book Header ( Old )
            Last Modified: 2010-01-08 02:06:00
            Modified By: Tomi

            Packet: 0x93
            Sent By: Both
            Size: 99 Bytes

            Packet Build
            BYTE[1] 0x93
            BYTE[4] Book Serial
            BYTE[1] Write Flag (see notes)
            BYTE[1] 0x1 (unknown)
            BYTE[2] Page Count
            BYTE[60] Title
            BYTE[30] Author

            Subcommand Build
            N/A

            Notes
            Write Flag
            0: Not Writable
            1: Writable

            Server version of packet is followed by packet 0x66 for Book Contents.

            Client sends a 0x93 message on book close. Update packet for the server to handle changes. Write Flag through Page Count are all 0's on client response
         */

        public BookHeaderOldPacket(PacketReader reader)
            : base(0x93, "Book Header (Old)")
        {

        }
    }
}
