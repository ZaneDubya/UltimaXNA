using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class LoginPacket : SendPacket
    {
        public LoginPacket(string username, string password)
            : base(0x80, "Account Login", 0x3E)
        {
            this.Stream.WriteAsciiFixed(username, 30);
            this.Stream.WriteAsciiFixed(password, 30);
            this.Stream.Write((byte)0x5D);
        }
    }
}
