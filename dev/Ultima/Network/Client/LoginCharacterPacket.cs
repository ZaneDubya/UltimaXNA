/***************************************************************************
 *   LoginCharacterPacket.cs
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
    /*
     * Client flags:
     * t2a: 0x00,
     * renaissance: 0x01,
     * third dawn: 0x02,
     * lbr: 0x04,
     * aos: 0x08,
     * se: 0x10,
     * sa: 0x20,
     * uo3d: 0x40,
     * reserved: 0x80,
     * 3dclient: 0x100
     */

    public class LoginCharacterPacket : SendPacket
    {
        public LoginCharacterPacket(string name, int index, int ipAddress)
            : base(0x5d, "Character Select", 0x49)
        {
            Stream.Write((uint)0xedededed); // pattern1
            Stream.WriteAsciiFixed(name, 30); // char name (0 terminated)
            Stream.Write((short)0); // unknown
            Stream.Write((int)0x1f); // clientflag. We should pick a unique flag for UltimaXNA.
            Stream.Write((int)1); // unknown1
            Stream.Write((int)0x18); //  logincount
            Stream.WriteAsciiFixed("", 0x10); // unknown2
            Stream.Write(index); // slot choosen
            Stream.Write(ipAddress); // clientIP
        }
    }
}
