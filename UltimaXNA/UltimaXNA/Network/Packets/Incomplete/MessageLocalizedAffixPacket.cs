using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class MessageLocalizedAffixPacket : RecvPacket
    {
        public MessageLocalizedAffixPacket(PacketReader reader)
            : base(0xCC, "Message Localized Affix")
        {
            // TODO: Write this packet.
        }
    }
}
