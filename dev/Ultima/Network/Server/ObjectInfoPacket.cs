/***************************************************************************
 *   ObjectInfoPacket.cs
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
    public class ObjectInfoPacket : RecvPacket
    {
        public readonly Serial Serial;
        public readonly ushort ItemID;
        public readonly ushort Amount;
        public readonly short X;
        public readonly short Y;
        public readonly sbyte Z;
        public readonly byte Direction;
        public readonly ushort Hue;
        public readonly byte Flags;

        public ObjectInfoPacket(PacketReader reader)
            : base(0x1A, "ObjectInfoPacket")
        {
            Serial = reader.ReadInt32();
            ItemID = reader.ReadUInt16();

            Amount = (ushort)(((Serial & 0x80000000) == 0x80000000) ? reader.ReadUInt16() : 0);

            X = reader.ReadInt16();
            Y = reader.ReadInt16();
                        
            Direction = (byte)(((X & 0x8000) == 0x8000) ? reader.ReadByte() : 0);

            Z = reader.ReadSByte();

            Hue = (ushort)(((Y & 0x8000) == 0x8000) ? reader.ReadUInt16() : 0);

            Flags = (byte)(((Y & 0x4000) == 0x4000) ? reader.ReadByte() : 0);

            // sanitize values
            Serial = (int)(Serial & 0x7FFFFFFF);
            ItemID = (ushort)(ItemID & 0x7FFF);
            X = (short)(X & 0x7FFF);
            Y = (short)(Y & 0x3FFF);
        }
    }
}
