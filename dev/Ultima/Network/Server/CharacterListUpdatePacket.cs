/***************************************************************************
 *   CharacterListUpdatePacket.cs
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
using UltimaXNA.Ultima.Login.Accounts;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class CharacterListUpdatePacket : RecvPacket
    {
        CharacterListEntry[] m_characters;
        public CharacterListEntry[] Characters
        {
            get { return m_characters; }
        }

        public CharacterListUpdatePacket(PacketReader reader)
            : base(0x86, "Character List Update")
        {
            // Documented at http://docs.polserver.com/packets/index.php?Packet=0xA8
            int characterCount = reader.ReadByte();
            m_characters = new CharacterListEntry[characterCount];

            for (int i = 0; i < characterCount; i++)
            {
                m_characters[i] = new CharacterListEntry(reader);
            }
        }
    }
}
