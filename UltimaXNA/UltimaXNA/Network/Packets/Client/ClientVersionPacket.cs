using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class ClientVersionPacket : SendPacket
    {
        public ClientVersionPacket(string version)
            : base(0xBD, "Client Version")
        {
            Stream.WriteAsciiNull(version);
        }
    }
}
