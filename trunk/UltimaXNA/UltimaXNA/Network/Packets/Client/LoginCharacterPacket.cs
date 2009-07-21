/***************************************************************************
 *   LoginCharacterPacket.cs
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
#endregion

namespace UltimaXNA.Network.Packets.Client
{
    public class LoginCharacterPacket : SendPacket
    {
        public LoginCharacterPacket(string name, int index, int ipAddress)
            : base(0x5d, "Character Select", 0x49)
        {
            Stream.Write((uint)0xedededed);
            Stream.WriteAsciiFixed(name, 30);
            Stream.Write((short)0);
            Stream.Write((int)0x1f);
            Stream.Write((int)1);
            Stream.Write((int)0x18);
            Stream.WriteAsciiFixed("", 0x10);
            Stream.Write(index);
            Stream.Write(ipAddress);
        }
    }
}
