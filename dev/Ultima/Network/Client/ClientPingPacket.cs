/***************************************************************************
 *   ClientPingPacket.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using UltimaXNA.Core.Network.Packets;
using UltimaXNA.Ultima.Resources;
#endregion

namespace UltimaXNA.Ultima.Network.Client
{
    public class ClientPingPacket : SendPacket
    {
        public ClientPingPacket()
            : base(0x73, "Ping Packet", 2)
        {
            Stream.Write((byte)0);
        }
    }
}
