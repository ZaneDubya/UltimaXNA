/***************************************************************************
 *   SeedPacket.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
#endregion

namespace UltimaXNA.Network.Packets.Client
{
    public class SeedPacket : SendPacket
    {
        public SeedPacket(int seed, int major, int minor, int revision, int prototype)
            : base(0xEF, "Seed", 21)
        {
            Stream.Write(seed);
            Stream.Write(major);
            Stream.Write(minor);
            Stream.Write(revision);
            Stream.Write(prototype);
        }
    }
}