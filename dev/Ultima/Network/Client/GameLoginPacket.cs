/***************************************************************************
 *   GameLoginPacket.cs
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
    public class GameLoginPacket : SendPacket
    {
        public GameLoginPacket(int authId, string username, string password)
            : base(0x91, "Game Server Login", 0x41)
        {
            Stream.Write(authId);
            Stream.WriteAsciiFixed(username, 30);
            Stream.WriteAsciiFixed(password, 30);
        }

    }
}
