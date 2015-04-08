/***************************************************************************
 *   SwingPacket.cs
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
    public class SwingPacket : RecvPacket
    {
        readonly Serial m_attacker;
        readonly Serial m_defender;
        readonly byte m_flag;

        public Serial Attacker
        {
            get { return m_attacker; }
        }

        public Serial Defender
        {
            get { return m_defender; }
        }

        public byte Flag
        {
            get { return m_flag; }
        }

        public SwingPacket(PacketReader reader)
            : base(0x2F, "Swing")
        {
            m_flag = reader.ReadByte();
            m_attacker = reader.ReadInt32();
            m_defender = reader.ReadInt32();
        }
    }
}
