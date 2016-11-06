/***************************************************************************
 *   ChatPacket.cs
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
    public class ChatPacket : RecvPacket
    {
        readonly string m_language;
        readonly byte m_commandtype;

        public string Language
        {
            get { return m_language; }
        }

        public byte CommandType
        {
            get { return m_commandtype; }
        } 

        public ChatPacket(PacketReader reader)
            : base(0xB3, "Chat Packet")
        {
            m_language = reader.ReadString(3);
            reader.ReadInt16(); // unknown.
            m_commandtype = reader.ReadByte();
        }
    }
}
