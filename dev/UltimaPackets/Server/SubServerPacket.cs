/***************************************************************************
 *   SubServerPacket.cs
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

namespace UltimaXNA.UltimaPackets.Server
{
    public class SubServerPacket : RecvPacket
    {
        readonly short m_x;
        readonly short m_y;
        readonly short m_z;
        readonly short m_mapWidth;
        readonly short m_mapHeight;

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

        public short MapWidth
        {
            get { return m_mapWidth; }
        }

        public short MapHeight
        {
            get { return m_mapHeight; }
        }

        public SubServerPacket(PacketReader reader)
            : base(0xB3, "Chat Packet")
        {
            m_x = reader.ReadInt16();
            m_y = reader.ReadInt16();
            m_z = reader.ReadInt16();
            reader.ReadByte();
            reader.ReadInt16();
            reader.ReadInt16();
            m_mapWidth = reader.ReadInt16();
            m_mapHeight = reader.ReadInt16();
        }
    }
}
