using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class SubServerPacket : RecvPacket
    {
        readonly short _x;
        readonly short _y;
        readonly short _z;
        readonly short _mapWidth;
        readonly short _mapHeight;

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

        public short MapWidth
        {
            get { return _mapWidth; }
        }

        public short MapHeight
        {
            get { return _mapHeight; }
        }

        public SubServerPacket(PacketReader reader)
            : base(0xB3, "Chat Packet")
        {
            _x = reader.ReadInt16();
            _y = reader.ReadInt16();
            _z = reader.ReadInt16();
            reader.ReadByte();
            reader.ReadInt16();
            reader.ReadInt16();
            _mapWidth = reader.ReadInt16();
            _mapHeight = reader.ReadInt16();
        }
    }
}
