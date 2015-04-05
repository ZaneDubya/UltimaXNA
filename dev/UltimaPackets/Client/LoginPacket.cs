/***************************************************************************
 *   LoginPacket.cs
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
#endregion

namespace UltimaXNA.UltimaPackets.Client
{
    public class LoginPacket : SendPacket
    {
        public LoginPacket(string username, string password)
            : base(0x80, "Account Login", 0x3E)
        {
            Stream.WriteAsciiFixed(username, 30);
            Stream.WriteAsciiFixed(password, 30);
            Stream.Write((byte)0x5D);
        }
    }
}
