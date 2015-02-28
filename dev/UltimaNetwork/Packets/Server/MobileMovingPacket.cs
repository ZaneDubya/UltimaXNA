/***************************************************************************
 *   MobileMovingPacket.cs
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
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.UltimaNetwork.Packets.Server
{
    public class MobileMovingPacket : RecvPacket
    {
        readonly Serial serial;
        readonly ushort bodyid;
        readonly short x;
        readonly short y;
        readonly sbyte z;
        readonly byte direction;
        readonly ushort hue;
        public readonly MobileFlags Flags;
        readonly byte notoriety;

        public Serial Serial 
        {
            get { return serial; }
        }

        public ushort BodyID 
        {
            get { return bodyid; }
        }

        public short X
        {
            get { return x; }
        }

        public short Y 
        {
            get { return y; } 
        }

        public sbyte Z 
        {
            get { return z; } 
        }

        public byte Direction
        {
            get { return direction; }
        }

        public ushort Hue
        {
            get { return hue; }
        }

        public byte Notoriety
        {
            get { return notoriety; }
        }

        public MobileMovingPacket(PacketReader reader)
            : base(0x77, "Mobile Moving")
        {
            this.serial = reader.ReadInt32();
            this.bodyid = reader.ReadUInt16();
            this.x = reader.ReadInt16();
            this.y = reader.ReadInt16();
            this.z = reader.ReadSByte();
            this.direction = reader.ReadByte();
            this.hue = reader.ReadUInt16();
            this.Flags = new MobileFlags(reader.ReadByte());
            this.notoriety = reader.ReadByte();
        }
    }
}
