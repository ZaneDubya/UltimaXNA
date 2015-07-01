/***************************************************************************
 *   WarModePacket.cs
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
    public class WarModePacket : RecvPacket
    {
        readonly byte m_warmode;

        public bool WarMode
        {
            get { return (m_warmode == 0x01); }
        }

        public WarModePacket(PacketReader reader)
            : base(0x72, "Request War Mode")
        {
            m_warmode = reader.ReadByte();

            reader.ReadByte(); // always 0x00
            reader.ReadByte(); // always 0x32
            reader.ReadByte(); // always 0x00
        }
    }
}
