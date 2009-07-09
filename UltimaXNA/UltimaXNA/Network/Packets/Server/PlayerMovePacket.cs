using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class PlayerMovePacket : RecvPacket
    {
        readonly byte _direction;

        public byte Direction
        {
            get { return _direction; }
        }

        public PlayerMovePacket(PacketReader reader)
            : base(0x97, "Player Move")
        {
            _direction = reader.ReadByte();
        }
    }
}
