/***************************************************************************
 *   ContainerPacket.cs
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
    public class OpenContainerPacket : RecvPacket
    {
        readonly Serial m_serial;
        readonly ushort m_gumpId;

        public Serial Serial { get { return m_serial; } }
        public ushort GumpId { get { return m_gumpId; } }

        public OpenContainerPacket(PacketReader reader)
            : base(0x24, "Open Container")
        {
            m_serial = reader.ReadInt32();
            m_gumpId = reader.ReadUInt16();
        }
    }
}
