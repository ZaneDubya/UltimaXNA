using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
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
        readonly byte flags;
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

        public byte Flags
        {
            get { return flags; } 
        }

        /// <summary>
        /// These are the only flags sent by RunUO
        /// 0x02 = female
        /// 0x04 = poisoned
        /// 0x08 = blessed/yellow health bar
        /// 0x40 = warmode
        /// 0x80 = hidden
        //// </summary>
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
            this.flags = reader.ReadByte();
            this.notoriety = reader.ReadByte();
        }
    }
}
