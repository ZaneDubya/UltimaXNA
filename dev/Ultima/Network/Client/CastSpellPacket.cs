/***************************************************************************
 *   CastSpellPacket.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.Ultima.Network.Client
{
    public class CastSpellPacket : SendPacket
    {
        public CastSpellPacket(int spellIndex)
            : base(0xBF, "Cast Spell")
        {
            Stream.Write((short)0x001C); // subcommand 0x1C - cast spell
            Stream.Write((short)0x0002); // unknown - always 2 in legacy client.
            Stream.Write((short)spellIndex);
        }
    }
}
