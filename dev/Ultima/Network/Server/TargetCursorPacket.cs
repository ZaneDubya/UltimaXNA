/***************************************************************************
 *   TargetCursorPacket.cs
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
    public class TargetCursorPacket : RecvPacket
    {
        public readonly byte CommandType;
        public readonly int CursorID;
        public readonly byte CursorType;
        
        public TargetCursorPacket(PacketReader reader)
            : base(0x6C, "Target Cursor")
        {
            CommandType = reader.ReadByte(); // 0x00 = Select Object; 0x01 = Select X, Y, Z
            CursorID = reader.ReadInt32();
            CursorType = reader.ReadByte(); // 0 - 2 = unknown; 3 = Cancel current targetting RunUO seems to always send 0.
        }
    }
}
