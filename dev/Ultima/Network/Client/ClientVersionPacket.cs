/***************************************************************************
 *   ClientVersionPacket.cs
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
using UltimaXNA.Core.Network.Packets;
using UltimaXNA.Core.Diagnostics.Tracing;

#endregion

namespace UltimaXNA.Ultima.Network.Client
{
    public class ClientVersionPacket : SendPacket
    {
        public ClientVersionPacket(byte[] version)
            : base(0xBD, "Client Version")
        {
            if (version.Length != 4)
                Tracer.Critical("SeedPacket: Incorrect length of version.");
            Stream.WriteAsciiNull(string.Format("{0}.{1}.{2}.{3}", version[0], version[1], version[2], version[3]));
        }
    }
}
