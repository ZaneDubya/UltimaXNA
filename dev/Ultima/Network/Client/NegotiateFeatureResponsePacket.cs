/***************************************************************************
 *   NegotiateFeatureResponsePacket.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
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
    class NegotiateFeatureResponsePacket : SendPacket
    {
        public NegotiateFeatureResponsePacket()
            : base(0xF0, "Negotiate Feature Response")
        {
            Stream.Write((byte)0xFF); // acknowledge handshake.
        }
    }
}
