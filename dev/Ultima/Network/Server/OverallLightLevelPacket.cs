/***************************************************************************
 *   OverallLightLevelPacket.cs
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
    public class OverallLightLevelPacket : RecvPacket
    {
        readonly byte m_lightLevel;

        public byte LightLevel
        {
            get { return m_lightLevel; }
        }

        public OverallLightLevelPacket(PacketReader reader)
            : base(0x4F, "OverallLightLevel")
        {
            m_lightLevel = reader.ReadByte();
        }
    }
}
