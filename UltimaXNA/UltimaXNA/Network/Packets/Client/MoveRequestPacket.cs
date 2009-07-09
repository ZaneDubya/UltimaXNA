using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class MoveRequestPacket : SendPacket
    {
        public MoveRequestPacket(byte direction, byte sequence, int fastWalkPreventionKey)
            : base(0x02, "Move Request", 7)
        {
            Stream.Write((byte)direction);
            Stream.Write((byte)sequence);
            Stream.Write(fastWalkPreventionKey);
        }
    }
}
