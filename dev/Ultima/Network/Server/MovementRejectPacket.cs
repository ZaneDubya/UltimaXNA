/***************************************************************************
 *   MovementRejectPacket.cs
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
    public class MovementRejectPacket : RecvPacket
    {
        readonly byte m_sequence;
        readonly short m_x;
        readonly short m_y;
        readonly byte m_direction;
        readonly sbyte m_z;

        public byte Sequence 
        {
            get { return m_sequence; } 
        }
        
        public short X
        {
            get { return m_x; }
        }

        public short Y 
        {
            get { return m_y; }
        }
        
        public byte Direction
        {
            get { return m_direction; }
        }
        
        public sbyte Z 
        {
            get { return m_z; } 
        }

        public MovementRejectPacket(PacketReader reader)
            : base(0x21, "Move Request Rejected")
        {
            m_sequence = reader.ReadByte(); // (matches sent sequence)
            m_x = reader.ReadInt16();
            m_y = reader.ReadInt16();
            m_direction = reader.ReadByte();
            m_z = reader.ReadSByte();
        }
    }
}
