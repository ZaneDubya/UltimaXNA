/***************************************************************************
 *   ResponseToDialogBoxPacket.cs
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
    public class ResponseToDialogBoxPacket : SendPacket
    {
        public ResponseToDialogBoxPacket(int dialogId, short menuId, short index, short modelNum, short color)
            : base(0x7D, "Response To Dialog Box", 13)
        {
            Stream.Write(dialogId);
            Stream.Write((short)menuId);
            Stream.Write((short)index);
            Stream.Write((short)modelNum);
            Stream.Write((short)color);
        }
    }
}
