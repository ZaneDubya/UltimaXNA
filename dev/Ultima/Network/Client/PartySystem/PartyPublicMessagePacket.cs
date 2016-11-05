/***************************************************************************
 *   PartyPublicMessagePacket.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client.PartySystem {
    public class PartyPublicMessagePacket : SendPacket {
        public PartyPublicMessagePacket(string text) 
            : base(0xbf, "Public Party Message") {
            Stream.Write((short)6);
            Stream.Write((byte)4);
            Stream.WriteBigUniNull(text);
            Stream.Write((short)0);
        }
    }
}