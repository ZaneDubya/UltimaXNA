/***************************************************************************
 *   SubServerPacket.cs
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
    public class SubServerPacket : RecvPacket
    {
        public readonly short X;
        public readonly short Y;
        public readonly short Z;
        public readonly short MapWidth;
        public readonly short MapHeight;

        public SubServerPacket(PacketReader reader)
            : base(0x76, "Move to subserver")
        {
            X = reader.ReadInt16();
            Y = reader.ReadInt16();
            Z = reader.ReadInt16();
            reader.ReadByte(); // unknown - always 0
            reader.ReadInt16(); // server boundary x
            reader.ReadInt16(); // server boundary y
            MapWidth = reader.ReadInt16();
            MapHeight = reader.ReadInt16();
        }
    }
}
