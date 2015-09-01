/***************************************************************************
 *   MoveRequestPacket.cs
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
    public class MoveRequestPacket : SendPacket
    {
        public MoveRequestPacket(byte direction, byte sequence, int fastWalkPreventionKey)
            : base(0x02, "Move Request", 7)
        {
            Stream.Write((byte)direction);
            Stream.Write((byte)sequence);
            Stream.Write(fastWalkPreventionKey);
        }
    }
}
