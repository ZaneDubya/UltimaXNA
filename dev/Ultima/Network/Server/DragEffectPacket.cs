/***************************************************************************
 *   DragEffectPacket.cs
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
    public class DragEffectPacket : RecvPacket
    {
        public readonly int ItemId;
        public readonly int Amount;
        public readonly Serial Source;
        public readonly int SourceX;
        public readonly int SourceY;
        public readonly int SourceZ;
        public readonly Serial Destination;
        public readonly int DestX;
        public readonly int DestY;
        public readonly int DestZ;

        public DragEffectPacket(PacketReader reader)
            : base(0x23, "Dragging Item")
        {
            ItemId = reader.ReadUInt16();
            reader.ReadByte(); // 0x03 bytes unknown.
            reader.ReadByte(); //
            reader.ReadByte(); //
            Amount = reader.ReadUInt16();
            Source = reader.ReadInt32(); // 0x00000000 or 0xFFFFFFFF for ground
            SourceX = reader.ReadUInt16();
            SourceY = reader.ReadUInt16();
            SourceZ = reader.ReadByte();
            Destination = reader.ReadInt32(); // 0x00000000 or 0xFFFFFFFF for ground
            DestX = reader.ReadUInt16();
            DestY = reader.ReadUInt16();
            DestZ = reader.ReadByte();
        }
    }
}
