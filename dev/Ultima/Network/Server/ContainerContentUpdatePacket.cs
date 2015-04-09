/***************************************************************************
 *   ContainerContentUpdatePacket.cs
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
    public class ContainerContentUpdatePacket : RecvPacket
    {
        readonly Serial m_serial;
        readonly int m_itemId;
        readonly int m_amount;
        readonly int m_x;
        readonly int m_y;
        readonly int m_gridLocation;
        readonly Serial m_parentSerial;
        readonly int m_hue;

        public int ItemId
        {
            get { return m_itemId; }
        }

        public int Amount
        {
            get { return m_amount; }
        }

        public int X
        {
            get { return m_x; }
        }

        public int Y
        {
            get { return m_y; }
        }

        public int GridLocation
        {
            get { return m_gridLocation; }
        }

        public Serial ContainerSerial
        {
            get { return m_parentSerial; }
        }

        public int Hue
        {
            get { return m_hue; }
        }

        public Serial Serial
        {
            get { return m_serial; }
        }

        public ContainerContentUpdatePacket(PacketReader reader)
            : base(0x25, "Add Single Item")
        {
            m_serial = reader.ReadInt32();
            m_itemId = reader.ReadUInt16();
            reader.ReadByte(); // unknown 
            m_amount = reader.ReadUInt16();
            m_x = reader.ReadInt16();
            m_y = reader.ReadInt16();
            m_gridLocation = reader.ReadByte(); // always 0 in RunUO.
            m_parentSerial = (Serial)reader.ReadInt32();
            m_hue = reader.ReadUInt16();
        }
    }
}
