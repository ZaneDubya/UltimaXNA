/***************************************************************************
 *   PlayerMovePacket.cs
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
    public class PlayerMovePacket : RecvPacket
    {
        readonly byte m_direction;

        public byte Direction
        {
            get { return m_direction; }
        }

        public PlayerMovePacket(PacketReader reader)
            : base(0x97, "Player Move")
        {
            m_direction = reader.ReadByte();
        }
    }
}
