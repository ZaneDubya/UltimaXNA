using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
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

            reader.ReadInt32();//unknown..

            _body = reader.ReadInt16();
            _x = reader.ReadInt16();
            _y = reader.ReadInt16();
            _z = reader.ReadInt16();
            _direction = reader.ReadByte();
        }
    }
}
