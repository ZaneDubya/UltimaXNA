/***************************************************************************
 *   ObjectHelpResponsePacket.cs
 *   Copyright (c) 2009 UltimaXNA Development Team
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
    public class ObjectHelpResponsePacket : RecvPacket
    {
        public ObjectHelpResponsePacket(PacketReader reader)
            : base(0xB7, "Display Menu")
        {
            // TODO: Write this packet.
        }
    }
}
