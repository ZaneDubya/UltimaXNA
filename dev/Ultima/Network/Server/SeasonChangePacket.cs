/***************************************************************************
 *   SeasonChangePacket.cs
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
using UltimaXNA.Ultima.Data;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    /// <summary>
    /// Seasonal Information packet.
    /// </summary>
    public class SeasonChangePacket : RecvPacket
    {
        public bool SeasonChanged
        {
            get;
            private set;
        }
        public Seasons Season
        {
            get;
            private set;
        }

        public SeasonChangePacket(PacketReader reader)
            : base(0xBC, "Seasonal Information")
        {
            Season = (Seasons)reader.ReadByte();
            SeasonChanged = reader.ReadByte() == 1;
        }
    }
}
