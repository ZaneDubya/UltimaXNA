using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network
{
    class QueuedPacket
    {
        public List<PacketHandler> PacketHandlers;
        public byte[] PacketBuffer;
        public int RealLength;
        public string Name;

        public QueuedPacket(string name, List<PacketHandler> packetHandlers, byte[] packetBuffer, int realLength)
        {
            Name = name;
            PacketHandlers = packetHandlers;
            PacketBuffer = packetBuffer;
            RealLength = realLength;
        }
    }
}
