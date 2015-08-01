/***************************************************************************
 *   ObjectInfoPacket.cs
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

namespace UltimaXNA.Ultima.Network.Server
{
    public class WorldItemPacket : RecvPacket
    {
        readonly Serial m_serial;
        readonly ushort m_itemid;
        readonly ushort m_amount;
        readonly short m_x;
        readonly short m_y;
        readonly sbyte m_z;
        readonly byte m_direction;
        readonly ushort m_hue;
        readonly byte m_flags;

        public Serial Serial 
        {
            get { return m_serial; } 
        }

        public ushort ItemID
        {
            get { return m_itemid; }
        }

        public ushort StackAmount 
        {
            get { return m_amount; } 
        }

        public short X 
        {
            get { return m_x; }
        }

        public short Y 
        {
            get { return m_y; }
        }

        public short Z 
        {
            get { return m_z; }
        }

        public byte Direction
        {
            get { return m_Direction; }
        }

        public ushort Hue 
        {
            get { return m_hue; }
        }

        public short Flags
        {
            get { return m_flags; }
        }

        public WorldItemPacket(PacketReader reader)
            : base(0x1A, "ObjectInfo")
        {
            Serial serial = reader.ReadInt32();
            ushort itemId = reader.ReadUInt16();

            m_amount = 0;

            if ((serial & 0x80000000) == 0x80000000)
            {
                m_amount = reader.ReadUInt16();
            }

            ushort x = reader.ReadUInt16();
            ushort y = reader.ReadUInt16();
                        
            m_direction = 0;

            if ((x & 0x8000) == 0x8000)
                m_direction = reader.ReadByte();

            m_z = reader.ReadSByte();
            m_hue = 0;

            if ((y & 0x8000) == 0x8000)
                m_hue = reader.ReadUInt16();

            m_flags = 0;

            if ((y & 0x4000) == 0x4000)
                m_flags = reader.ReadByte();

            m_serial = (int)(serial &= 0x7FFFFFFF);
            m_itemid = (ushort)(itemId &= 0x7FFF);
            m_x = (short)(x &= 0x7FFF);
            m_y = (short)(y &= 0x3FFF);
        }

        public byte m_Direction { get; set; }
    }
}
