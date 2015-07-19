/***************************************************************************
 *   GuildGumpRequestPacket.cs
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
    public class GuildGumpRequestPacket : SendPacket
    {
        public GuildGumpRequestPacket(Serial serial)
            : base(0xD7, "Guild gump request")
        {
            Stream.Write(serial);
            Stream.Write((ushort)0x0028);
            Stream.Write((byte)0x0A);
        }
    }
}
