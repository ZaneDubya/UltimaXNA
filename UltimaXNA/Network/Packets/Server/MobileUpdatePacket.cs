/***************************************************************************
 *   MobileUpdatePacket.cs
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
    public class MobileUpdatePacket : RecvPacket
    {
        readonly Serial _serial;
        readonly short _body;
        readonly short _x;
        readonly short _y;
        readonly short _z;
        readonly byte _direction;
        readonly ushort _hue;
        readonly MobileFlags _flags;

        public Serial Serial
        {
            get { return _serial; }
        }

        public short BodyID
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

        public ushort Hue         
        { 
            get { return _hue; }
        }

        public MobileFlags Flags
        {
            get { return _flags; }
        } 

        public MobileUpdatePacket(PacketReader reader)
            : base(0x20, "Mobile Update")
        {
            _serial = reader.ReadInt32();
            _body = reader.ReadInt16();
            reader.ReadByte(); // Always 0
            _hue = reader.ReadUInt16(); // Skin hue
            _flags = new MobileFlags(reader.ReadByte());
            _x = reader.ReadInt16();
            _y = reader.ReadInt16();
            reader.ReadInt16(); // Always 0
            _direction = reader.ReadByte();
            _z = reader.ReadSByte();
        }
    }
}
