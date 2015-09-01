/***************************************************************************
 *   SeedPacket.cs
 *   Copyright (c) 2009 UltimaXNA Development Team
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.Ultima.Network.Client
{
    public class SeedPacket : SendPacket
    {
        public SeedPacket(int seed, byte[] version)
            : base(0xEF, "Seed", 21)
        {
            Stream.Write(seed);
            if (version.Length != 4)
                Tracer.Critical("SeedPacket: version array is not the correct length (4).");
            Stream.Write((int)version[0]);
            Stream.Write((int)version[1]);
            Stream.Write((int)version[2]);
            Stream.Write((int)version[3]);
        }
    }
}