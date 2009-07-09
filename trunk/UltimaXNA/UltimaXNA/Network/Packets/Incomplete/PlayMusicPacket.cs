using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class PlayMusicPacket : RecvPacket
    {
        public PlayMusicPacket(PacketReader reader)
            : base(0x6D, "Play Music")
        {

        }
    }
}
