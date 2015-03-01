/***************************************************************************
 *   LoginConfirmPacket.cs
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

namespace UltimaXNA.UltimaPackets.Server
{
    public class LoginConfirmPacket : RecvPacket
    {
        readonly Serial _serial;
        readonly short _body;
        readonly short _x;
        readonly short _y;
        readonly short _z;
        readonly byte _direction;

        public Serial Serial
        {
            get { return _serial; }
        }

        public short Body
        {
            get { return _body; }
        }

        public short X
        {
            get { return _x; }
        }

        public short Y
        {
            get { return _y; }
        }

        public short Z
        {
            get { return _z; }
        }

        public byte Direction
        {
            get { return _direction; }
        }

        public LoginConfirmPacket(PacketReader reader)
            : base(0x1B, "Login Confirm")
        {
            _serial = reader.ReadInt32();

            reader.ReadInt32(); //unknown. Always 0.

            _body = reader.ReadInt16();
            _x = reader.ReadInt16();
            _y = reader.ReadInt16();
            _z = reader.ReadInt16();
            _direction = reader.ReadByte();
        }
    }
}
