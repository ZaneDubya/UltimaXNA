/***************************************************************************
 *   TimePacket.cs
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
    public class TimePacket : RecvPacket
    {
        readonly byte m_hour, m_minute, m_second;
        public byte Hour { get { return m_hour; } }
        public byte Minute { get { return m_minute; } }
        public byte Second { get { return m_second; } }

        public TimePacket(PacketReader reader)
            : base(0x5B, "Time")
        {
            m_hour = reader.ReadByte();
            m_minute = reader.ReadByte();
            m_second = reader.ReadByte();
        }
    }
}
