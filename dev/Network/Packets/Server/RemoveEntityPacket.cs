/***************************************************************************
 *   RemoveEntityPacket.cs
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
using UltimaXNA.Network;
#endregion

namespace UltimaXNA.Network.Packets.Server
{
    public class RemoveEntityPacket : RecvPacket
    {
        readonly Serial _serial;

        public Serial Serial
        {
            get { return _serial; }
        }

        public RemoveEntityPacket(PacketReader reader)
            : base(0x1D, "Remove Entity")
        {
            _serial = reader.ReadInt32();
        }
    }
}
