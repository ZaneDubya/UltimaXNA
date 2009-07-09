using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class LoginRejectionPacket : RecvPacket
    {
        public static string[] Reasons = new string[] {
              "Incorrect username and/or password.",
              "Someone is already using this account.",
              "Your account has been blocked.",
              "Your account credentials are invalid.",
              "Communication problem.",
              "The IGR concurrency limit has been met.",
              "The IGR time limit has been met.",
              "General IGR authentication failure."
            };

        byte _id;

        public string Reason
        {
            get { return LoginRejectionPacket.Reasons[_id]; }
        }

        public LoginRejectionPacket(PacketReader reader)
            : base(0x82, "Login Rejection")
        {
            _id = reader.ReadByte();
        }
    }
}
