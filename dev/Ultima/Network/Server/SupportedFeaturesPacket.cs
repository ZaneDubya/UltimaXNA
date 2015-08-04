/***************************************************************************
 *   SupportedFeaturesPacket.cs
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
using UltimaXNA.Ultima.Data;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class SupportedFeaturesPacket : RecvPacket
    {
        readonly FeatureFlags m_flags;

        /// <summary>
        /// From POLServer packet docs: http://docs.polserver.com/packets/index.php?Packet=0xB9
        /// 0x01: enable T2A features: chat, regions
        /// 0x02: enable renaissance features
        /// 0x04: enable third dawn features
        /// 0x08: enable LBR features: skills, map
        /// 0x10: enable AOS features: skills, map, spells, fightbook
        /// 0x20: 6th character slot
        /// 0x40: enable SE features
        /// 0x80: enable ML features: elven race, spells, skills
        /// 0x100: enable 8th age splash screen
        /// 0x200: enable 9th age splash screen
        /// 0x400: enable 10th age
        /// 0x800: enable increased housing and bank storage
        /// 0x1000: 7th character slot
        /// 0x2000: 10th age KR faces
        /// 0x4000: enable trial account
        /// 0x8000: enable 11th age
        /// 0x10000: enable SA features: gargoyle race, spells, skills
        /// </summary>
        public FeatureFlags Flags
        {
            get { return m_flags; }
        }

        public SupportedFeaturesPacket(PacketReader reader)
            : base(0xB9, "Enable Features")
        {
            m_flags = (FeatureFlags)reader.ReadInt16();
        }
    }
}
