using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class MovementRejectPacket : RecvPacket
    {
        readonly byte _sequence;
        readonly short _x;
        readonly short _y;
        readonly byte _direction;
        readonly byte _z;

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
        
        public byte Z 
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
            _z = reader.ReadByte();
        }
    }
}
