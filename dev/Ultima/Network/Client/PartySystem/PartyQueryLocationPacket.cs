/***************************************************************************
 *   PartyQueryLocationPacket.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client.PartySystem {
    public class PartyQueryLocationPacket : SendPacket {
        public PartyQueryLocationPacket() 
            : base(240, "Query Party Locations") {
            Stream.Write((byte)0);
        }
    }
}