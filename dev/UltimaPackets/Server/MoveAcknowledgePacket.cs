/***************************************************************************
 *   MoveAcknowledgePacket.cs
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
    public class MoveAcknowledgePacket : RecvPacket
    {
        readonly byte m_sequence;
        readonly byte m_notoriety;

        public byte Sequence 
        {
            get { return m_sequence; } 
        }

        public byte Notoriety
        {
            get { return m_notoriety; }
        }

        public MoveAcknowledgePacket(PacketReader reader)
            : base(0x22, "Move Request Acknowledged")
        {
            m_sequence = reader.ReadByte(); // (matches sent sequence)
            m_notoriety = reader.ReadByte(); // Not sure why it sends this.
        }
    }
}
