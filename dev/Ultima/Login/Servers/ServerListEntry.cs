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

namespace UltimaXNA.Ultima.Login.Servers
{
    public class ServerListEntry
    {
        readonly ushort index;
        readonly string name;
        readonly byte percentFull;
        readonly byte timezone;
        readonly uint address;

        public ushort Index
        {
            get { return index; }
        }

        public string Name
        {
            get { return name; }
        }

        public byte PercentFull
        {
            get { return percentFull; }
        }

        public byte Timezone
        {
            get { return timezone; }
        }

        public uint Address
        {
            get { return address; }
        }

        public ServerListEntry(PacketReader reader)
        {
            index = (ushort)reader.ReadInt16();
            name = reader.ReadString(30);
            percentFull = reader.ReadByte();
            timezone = reader.ReadByte();
            address = (uint)reader.ReadInt32();
        }
    }
}
