/***************************************************************************
 *   ObjectInfoPacket.cs
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

namespace UltimaXNA.Network.Packets.Server
{
    public class WorldItemPacket : RecvPacket
    {
        readonly Serial _serial;
        readonly short _itemid;
        readonly short _amount;
        readonly short _x;
        readonly short _y;
        readonly sbyte _z;
        readonly byte _direction;
        readonly ushort _hue;
        readonly byte _flags;

        public Serial Serial 
        {
            get { return _serial; } 
        }

        public short ItemID
        {
            get { return _itemid; }
        }

        public short StackAmount 
        {
            get { return _amount; } 
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

        public short Flags
        {
            get { return _flags; }
        }

        public WorldItemPacket(PacketReader reader)
            : base(0x1A, "ObjectInfo")
        {
            Serial serial = reader.ReadInt32();
            ushort itemId = reader.ReadUInt16();

            _amount = 0;

            if ((serial & 0x80000000) == 0x80000000)
            {
                _amount = reader.ReadInt16();
            }
            
            // Doesn't exist this thing in the packet
            /*byte iIncrement = 0;
            if ((iItemID & 0x8000) == 0x8000)
            {
                iIncrement = reader.ReadByte();
                iObjectSerial += iIncrement;
            }*/

            ushort x = reader.ReadUInt16();
            ushort y = reader.ReadUInt16();
                        
            _direction = 0;

            if ((x & 0x8000) == 0x8000)
                _direction = reader.ReadByte();

            _z = reader.ReadSByte();
            _hue = 0;

            if ((y & 0x8000) == 0x8000)
                _hue = reader.ReadUInt16();

            _flags = 0;

            if ((y & 0x4000) == 0x4000)
                _flags = reader.ReadByte();

            _serial = (int)(serial &= 0x7FFFFFFF);
            _itemid = (short)(itemId &= 0x7FFF);
            _x = (short)(x &= 0x7FFF);
            _y = (short)(y &= 0x3FFF);
        }
    }
}
