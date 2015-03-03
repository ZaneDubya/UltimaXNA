/***************************************************************************
 *   WornItemPacket.cs
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

    public class WornItemPacket : RecvPacket
    {
        readonly Serial m_serial;
        readonly short m_itemId;
        readonly byte m_layer;
        readonly Serial m_parentSerial;
        readonly short m_hue;

        public Serial Serial
        {
            get { return m_serial; }
        }

        public short ItemId
        {
            get { return m_itemId; }
        }

        public byte Layer
        {
            get { return m_layer; }
        }

        public Serial ParentSerial
        {
            get { return m_parentSerial; }
        }

        public short Hue
        {
            get { return m_hue; }
        }


        public WornItemPacket(PacketReader reader)
            : base(0x2E, "Worn Item")
        {
            m_serial = reader.ReadInt32();
            m_itemId = reader.ReadInt16();
            reader.ReadByte();
            m_layer = reader.ReadByte();
            m_parentSerial = reader.ReadInt32();
            m_hue = reader.ReadInt16();
        }
    }
}
