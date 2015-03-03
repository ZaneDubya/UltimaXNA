/***************************************************************************
 *   DamagePacket.cs
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
    public class DamagePacket : RecvPacket
    {
        readonly Serial m_serial;
        readonly short m_damage;

        public Serial Serial
        {
            get { return m_serial; }
        } 

        public short Damage
        {
            get { return m_damage; }
        } 
        
        public DamagePacket(PacketReader reader)
            : base(0x0B, "Damage")
        {
            m_serial = reader.ReadInt32();
            m_damage = reader.ReadInt16();
        }
    }
}
