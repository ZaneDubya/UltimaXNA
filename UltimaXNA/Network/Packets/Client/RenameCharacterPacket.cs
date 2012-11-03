/***************************************************************************
 *   RenameCharacterPacket.cs
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
    public class RenameCharacterPacket : SendPacket
    {
        public RenameCharacterPacket(Serial serial, string name)
            : base(0x75, "Rename Request", 35)
        {
            Stream.Write(serial);
            Stream.WriteAsciiFixed(name, 30);
        }
    }
}
