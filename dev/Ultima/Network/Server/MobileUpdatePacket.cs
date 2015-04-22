/***************************************************************************
 *   MobileUpdatePacket.cs
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
using UltimaXNA.Ultima.Entities.Mobiles;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class MobileUpdatePacket : RecvPacket
    {
        readonly Serial m_serial;
        readonly short m_body;
        readonly short m_x;
        readonly short m_y;
        readonly short m_z;
        readonly byte m_direction;
        readonly ushort m_hue;
        readonly MobileFlags m_flags;

        public Serial Serial
        {
            get { return m_serial; }
        }

        public short BodyID
        {
            get { return m_body; } 
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
            get { return m_direction; } 
        }

        public ushort Hue         
        { 
            get { return m_hue; }
        }

        public MobileFlags Flags
        {
            get { return m_flags; }
        } 

        public MobileUpdatePacket(PacketReader reader)
            : base(0x20, "Mobile Update")
        {
            m_serial = reader.ReadInt32();
            m_body = reader.ReadInt16();
            reader.ReadByte(); // Always 0
            m_hue = reader.ReadUInt16(); // Skin hue
            m_flags = new MobileFlags((MobileFlag)reader.ReadByte());
            m_x = reader.ReadInt16();
            m_y = reader.ReadInt16();
            reader.ReadInt16(); // Always 0
            m_direction = reader.ReadByte();
            m_z = reader.ReadSByte();
        }
    }
}
