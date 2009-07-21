/***************************************************************************
 *   ServerListEntry.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
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
using System.Linq;
using System.Text;
#endregion

namespace UltimaXNA.Network
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
            this.index = (ushort)reader.ReadInt16();
            this.name = reader.ReadString(30);
            this.percentFull = reader.ReadByte();
            this.timezone = reader.ReadByte();
            this.address = (uint)reader.ReadInt32();
        }
    }
}
