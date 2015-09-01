/***************************************************************************
 *   RemoveEntityPacket.cs
 *   Copyright (c) 2009 UltimaXNA Development Team
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
    public class RemoveEntityPacket : RecvPacket
    {
        readonly Serial m_serial;

        public Serial Serial
        {
            get { return m_serial; }
        }

        public RemoveEntityPacket(PacketReader reader)
            : base(0x1D, "Remove Entity")
        {
            m_serial = reader.ReadInt32();
        }
    }
}
