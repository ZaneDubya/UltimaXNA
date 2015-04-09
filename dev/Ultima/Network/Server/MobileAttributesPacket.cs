/***************************************************************************
 *   MobileAttributesPacket.cs
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
    public class MobileAttributesPacket : RecvPacket
    {
        readonly Serial m_serial;
        readonly short m_maxHits;
        readonly short m_currentHits;
        readonly short m_maxMana;
        readonly short m_currentMana;
        readonly short m_maxStamina;
        readonly short m_currentStamina;

        public Serial Serial
        {
            get { return m_serial; }
        }

        public short MaxHits
        {
            get { return m_maxHits; }
        }

        public short CurrentHits
        {
            get { return m_currentHits; }
        }

        public short MaxMana
        {
            get { return m_maxMana; }
        }

        public short CurrentMana
        {
            get { return m_currentMana; }
        }

        public short MaxStamina
        {
            get { return m_maxStamina; }
        }

        public short CurrentStamina
        {
            get { return m_currentStamina; }
        }


        public MobileAttributesPacket(PacketReader reader)
            : base(0x2D, "Mobile Attributes")
        {
            m_serial = reader.ReadInt32();
            m_maxHits = reader.ReadInt16();
            m_currentHits = reader.ReadInt16();
            m_maxMana = reader.ReadInt16();
            m_currentMana = reader.ReadInt16();
            m_maxStamina = reader.ReadInt16();
            m_currentStamina = reader.ReadInt16();
        }
    }
}
