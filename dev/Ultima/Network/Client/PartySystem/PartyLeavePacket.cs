/***************************************************************************
 *   PartyLeavePacket.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using UltimaXNA.Core.Network.Packets;
using UltimaXNA.Ultima.World;

namespace UltimaXNA.Ultima.Network.Client.PartySystem {
    public class PartyLeavePacket : SendPacket {
        public PartyLeavePacket() 
            : base(0xbf, "Leave Party") {
            Stream.Write((short)6);
            Stream.Write((byte)2);
            Stream.Write(WorldModel.Entities.GetPlayerEntity().Serial);
        }
    }
}