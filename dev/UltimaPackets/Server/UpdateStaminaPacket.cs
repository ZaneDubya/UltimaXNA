/***************************************************************************
 *   UpdateStaminaPacket.cs
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
    public class UpdateStaminaPacket : RecvPacket
    {
        readonly Serial m_serial;
        readonly short m_current;
        readonly short m_max;

        public Serial Serial
        {
            get { return m_serial; }
        }

        public short Current
        {
            get { return m_current; }
        }

        public short Max
        {
            get { return m_max; } 
        }
        
        public UpdateStaminaPacket(PacketReader reader)
            : base(0xA3, "Update Stamina")
        {
            m_serial = reader.ReadInt32();
            m_max = reader.ReadInt16();
            m_current = reader.ReadInt16();
        }
    }
}
