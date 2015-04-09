/***************************************************************************
 *   PersonalLightLevelPacket.cs
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
    public class PersonalLightLevelPacket : RecvPacket
    {
        readonly Serial m_creatureSerial;
        readonly byte m_lightLevel;

        public Serial CreatureSerial
        {
            get { return m_creatureSerial; }
        }

        public byte LightLevel
        {
            get { return m_lightLevel; }
        }

        public PersonalLightLevelPacket(PacketReader reader)
            : base(0x4E, "Personal Light Level")
        {
            m_creatureSerial = reader.ReadInt32();
            m_lightLevel = reader.ReadByte();
        }
    }
}
