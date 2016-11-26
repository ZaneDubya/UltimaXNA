/***************************************************************************
 *   ServerListEntry.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Network;
#endregion

namespace UltimaXNA.Ultima.Login.Data {
    public class ServerListEntry {
        public readonly ushort Index;
        public readonly string Name;
        public readonly byte PercentFull;
        public readonly byte Timezone;
        public readonly uint Address;

        public ServerListEntry(PacketReader reader) {
            Index = (ushort)reader.ReadInt16();
            Name = reader.ReadString(32);
            PercentFull = reader.ReadByte();
            Timezone = reader.ReadByte();
            Address = (uint)reader.ReadInt32();
        }
    }
}
