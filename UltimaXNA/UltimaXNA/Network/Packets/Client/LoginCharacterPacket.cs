using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class LoginCharacterPacket : SendPacket
    {
        public LoginCharacterPacket(string name, int index, int ipAddress)
            : base(0x5d, "Character Select", 0x49)
        {
            Stream.Write((uint)0xedededed);
            Stream.WriteAsciiFixed(name, 30);
            Stream.Write((short)0);
            Stream.Write((int)0x1f);
            Stream.Write((int)1);
            Stream.Write((int)0x18);
            Stream.WriteAsciiFixed("", 0x10);
            Stream.Write(index);
            Stream.Write(ipAddress);
        }
    }
}
