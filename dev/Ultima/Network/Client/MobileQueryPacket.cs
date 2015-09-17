/***************************************************************************
 *   MobileQueryPacket.cs
 *   Copyright (c) 2009 UltimaXNA Development Team
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
    public class MobileQueryPacket : SendPacket
    {
        public enum StatusType : byte
        {
            GodClient = 0x00,
            BasicStatus = 0x04,
            Skills = 0x05
        }

        public MobileQueryPacket(StatusType type, Serial serial)
            : base(0x34, "Get Player Status", 10)
        {
            Stream.Write(0xEDEDEDED); // always 0xEDEDEDED in legacy client
            Stream.Write((byte)type);
            Stream.Write(serial);
        }
    }
}
