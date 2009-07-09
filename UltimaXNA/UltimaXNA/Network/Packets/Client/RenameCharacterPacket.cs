using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class RenameCharacterPacket : SendPacket
    {
        public RenameCharacterPacket(Serial serial, string name)
            : base(0x75, "Rename Request", 35)
        {
            Stream.Write(serial);
            Stream.WriteAsciiFixed(name, 30);
        }
    }
}
