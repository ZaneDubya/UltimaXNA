/***************************************************************************
 *   PartyLocationQueryPacket.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client.Extensions {
    /// <summary>
    /// MapUO Protocol: Requests the position of all party members.
    /// </summary>
    public class PartyLocationQueryPacket : SendPacket {
        public PartyLocationQueryPacket() 
            : base(0xF0, "Query Party Member Locations") {
            Stream.Write((byte)0);
        }
    }
}