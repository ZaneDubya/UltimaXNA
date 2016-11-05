/***************************************************************************
 *   PartyCanLootPacket.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client.PartySystem {
    class PartyCanLootPacket : SendPacket {
        public PartyCanLootPacket(bool isLootable) 
            : base(0xbf, "Party Can Loot") {
            Stream.Write((short)6);
            Stream.Write((byte)6);
            Stream.Write(isLootable);
        }
    }
}