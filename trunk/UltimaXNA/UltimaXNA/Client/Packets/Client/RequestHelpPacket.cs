/***************************************************************************
 *   RequestHelpPacket.cs
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

namespace UltimaXNA.Client.Packets.Client
{
    public class RequestHelpPacket : SendPacket
    {
        public RequestHelpPacket()
            : base(0x9B, "Request Help", 258)
        {
            byte[] empty = new byte[257];
            Stream.Write(empty, 0, 257);
        }
    }
}
