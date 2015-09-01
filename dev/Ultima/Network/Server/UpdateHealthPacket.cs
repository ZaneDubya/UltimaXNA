/***************************************************************************
 *   UpdateHealthPacket.cs
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
    public class UpdateHealthPacket : RecvPacket
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

        public UpdateHealthPacket(PacketReader reader)
            : base(0xA1, "Update Health")
        {
            m_serial = reader.ReadInt32();
            m_max = reader.ReadInt16();
            m_current = reader.ReadInt16();
        }
    }
}
