/***************************************************************************
 *   TimePacket.cs
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
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.UltimaNetwork.Packets.Server
{
    public class TimePacket : RecvPacket
    {
        readonly byte _hour, _minute, _second;
        public byte Hour { get { return _hour; } }
        public byte Minute { get { return _minute; } }
        public byte Second { get { return _second; } }

        public TimePacket(PacketReader reader)
            : base(0x5B, "Time")
        {
            _hour = reader.ReadByte();
            _minute = reader.ReadByte();
            _second = reader.ReadByte();
        }
    }
}
