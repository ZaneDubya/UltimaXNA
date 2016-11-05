/***************************************************************************
 *   PartyQueryStatsPacket.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client.PartySystem {
    public class PartyQueryStatsPacket : SendPacket {
        public PartyQueryStatsPacket(int Serial) 
            : base(0x34, "Query Stats", 10) {
            Stream.Write(0xFFFFFFFF);
            Stream.Write(0xDEDEDEDE);
            Stream.Write((byte)4);
            Stream.Write(Serial);
        }
    }
}