/***************************************************************************
 *   PartyAcceptPacket.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client.Partying {
    public class PartyAcceptPacket : SendPacket {
        public PartyAcceptPacket(Serial invitingPartyLeader) 
            : base(0xbf, "Party Join Accept") {
            Stream.Write((short)6);
            Stream.Write((byte)8);
            Stream.Write(invitingPartyLeader);
        }
    }
}