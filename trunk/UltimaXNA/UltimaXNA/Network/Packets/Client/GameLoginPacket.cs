using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class GameLoginPacket : SendPacket
    {
        public GameLoginPacket(int authId, string username, string password)
            : base(0x91, "Game Server Login", 0x41)
        {
            Stream.Write(authId);
            Stream.WriteAsciiFixed(username, 30);
            Stream.WriteAsciiFixed(password, 30);
        }

    }
}
