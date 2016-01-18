/***************************************************************************
 *   RequestNamePacket.cs
 *   Copyright (c) 2009 UltimaXNA Development Team
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
    public class RequestNamePacket : SendPacket
    {
        public RequestNamePacket(Serial serial)
            : base(0x98, "Request Name", 7)
        {
            Stream.Write((ushort)7);//This is not a fixed size packet! This is why there is the double size write.
            Stream.Write((int)serial);
        }
    }
}
