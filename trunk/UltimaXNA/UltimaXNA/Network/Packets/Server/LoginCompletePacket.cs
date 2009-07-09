using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class LoginCompletePacket : RecvPacket
    {
        public LoginCompletePacket(PacketReader reader)
            : base(0x55, "Login Complete")
        {

        }
    }
}
