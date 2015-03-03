/***************************************************************************
 *   CharacterListUpdatePacket.cs
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
using UltimaXNA.UltimaVars;
#endregion

namespace UltimaXNA.UltimaPackets.Server
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
