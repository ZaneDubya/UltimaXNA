using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class CharacterListUpdatePacket : RecvPacket
    {
        public CharacterListUpdatePacket(PacketReader reader)
            : base(0x86, "Character List Update")
        {
            // TODO: Write this packet.
        }
    }
}
