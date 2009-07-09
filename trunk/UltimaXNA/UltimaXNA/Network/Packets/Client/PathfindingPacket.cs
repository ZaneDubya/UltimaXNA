using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class PathfindingPacket : SendPacket
    {
        public PathfindingPacket(short x, short y, short z)
            : base(0x38, "Pathfinding", 7)
        {
            Stream.Write((short)x);
            Stream.Write((short)y);
            Stream.Write((short)z);
        }
    }
}
