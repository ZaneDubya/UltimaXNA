/***************************************************************************
 *   MovementRejectPacket.cs
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
    public class MovementRejectPacket : RecvPacket
    {
        readonly byte _sequence;
        readonly short _x;
        readonly short _y;
        readonly byte _direction;
        readonly sbyte _z;

        public byte Sequence 
        {
            get { return _sequence; } 
        }
        
        public short X
        {
            get { return _x; }
        }

        public short Y 
        {
            get { return _y; }
        }
        
        public byte Direction
        {
            get { return _direction; }
        }
        
        public sbyte Z 
        {
            get { return _z; } 
        }

        public MovementRejectPacket(PacketReader reader)
            : base(0x21, "Move Request Rejected")
        {
            _sequence = reader.ReadByte(); // (matches sent sequence)
            _x = reader.ReadInt16();
            _y = reader.ReadInt16();
            _direction = reader.ReadByte();
            _z = reader.ReadSByte();
        }
    }
}
