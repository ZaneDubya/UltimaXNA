using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class DeleteCharacterPacket : SendPacket
    {
        public DeleteCharacterPacket(string password, int characterIndex, int clientIp)
            : base(0x83, "Delete Character", 39)
        {
            Stream.WriteAsciiFixed(password, 30);
            Stream.Write(characterIndex);
            Stream.Write(clientIp);
        }
    }
}
