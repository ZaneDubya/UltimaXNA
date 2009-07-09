using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class MessageLocalizedPacket : RecvPacket
    {
        public MessageLocalizedPacket(PacketReader reader)
            : base(0xC1, "Message Localized")
        {
            short packetLength = reader.ReadInt16();
            int id = reader.ReadInt32(); // 0xffff for system message
            int body = reader.ReadInt16(); // (0xff for system message
            int type = reader.ReadByte(); // 6 - lower left, 7 on player
            int hue = reader.ReadUInt16();
            int font = reader.ReadInt16();
            int messageNumber = reader.ReadInt32();
            string speakerName = reader.ReadString(30);
            // what about the arguments?
            // http://docs.polserver.com/packets/index.php?Packet=0xC1
        }
    }
}
