/***************************************************************************
 *   LoginConfirmPacket.cs
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
    public class LoginConfirmPacket : RecvPacket
    {
        readonly Serial m_serial;
        readonly short m_body;
        readonly short m_x;
        readonly short m_y;
        readonly short m_z;
        readonly byte m_direction;

        public Serial Serial
        {
            get { return m_serial; }
        }

        public short Body
        {
            get { return m_body; }
        }

        public short X
        {
            get { return m_x; }
        }

        public short Y
        {
            get { return m_y; }
        }

        public short Z
        {
            get { return m_z; }
        }

        public byte Direction
        {
            get { return m_direction; }
        }

        public LoginConfirmPacket(PacketReader reader)
            : base(0x1B, "Login Confirm")
        {
            m_serial = reader.ReadInt32();

            reader.ReadInt32(); //unknown. Always 0.

            m_body = reader.ReadInt16();
            m_x = reader.ReadInt16();
            m_y = reader.ReadInt16();
            m_z = reader.ReadInt16();
            m_direction = reader.ReadByte();
        }
    }
}
