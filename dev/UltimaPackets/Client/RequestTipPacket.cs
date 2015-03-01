/***************************************************************************
 *   RequestTipPacket.cs
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
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.UltimaPackets.Client
{
    public class RequestTipPacket : SendPacket
    {
        public RequestTipPacket(short lastTipNumber)
            : base(0xA7, "Request Tip", 4)
        {
            Stream.Write((short)lastTipNumber);
            Stream.Write((byte)0x00);
        }
    }
}
