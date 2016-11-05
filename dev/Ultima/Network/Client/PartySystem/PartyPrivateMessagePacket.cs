/***************************************************************************
 *   PartyPrivateMessagePacket.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client.PartySystem {
    public class PartyPrivateMessagePacket : SendPacket {
        public PartyPrivateMessagePacket(Serial memberSerial, string msg) 
            : base(0xbf, "Private Party Message") {
            Stream.Write((short)6);
            Stream.Write((byte)3);
            Stream.Write(memberSerial);
            Stream.WriteBigUniNull(msg);
            Stream.Write((short)0);
        }
    }
}