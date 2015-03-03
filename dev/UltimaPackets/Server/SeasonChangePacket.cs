/***************************************************************************
 *   SeasonChangePacket.cs
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
    public class SeasonChangePacket : RecvPacket
    {
        readonly byte m_season;
        readonly byte m_playSound;

        public byte Season
        {
            get { return m_season; }
        }
        public byte PlaySound
        {
            get { return m_playSound; }
        }

        public SeasonChangePacket(PacketReader reader)
            : base(0xBC, "Seasonal Information")
        {
            m_season = reader.ReadByte();
            m_playSound = reader.ReadByte();
        }
    }
}
