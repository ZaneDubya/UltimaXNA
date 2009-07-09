using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class DisconnectNotificationPacket : SendPacket
    {
        public DisconnectNotificationPacket(Serial followed, Serial follower)
            : base(0x15, "Disconnect Notification", 9)
        {
            Stream.Write(followed);
            Stream.Write(follower);
        }
    }
}
