using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class MessageLocalizedPacket : RecvPacket
    {
        public bool IsSystemMessage { get { return (Serial == 0xFFFF); } }
        public readonly Serial Serial;
        public readonly int Body;
        public readonly int MessageType;
        public readonly int Hue;
        public readonly int Font;
        public readonly int CliLocNumber;
        public readonly string SpeakerName;
        public readonly string Arguements;

        public MessageLocalizedPacket(PacketReader reader)
            : base(0xC1, "Message Localized")
        {
            Serial = reader.ReadInt32(); // 0xffff for system message
            Body = reader.ReadInt16(); // (0xff for system message
            MessageType = reader.ReadByte(); // 6 - lower left, 7 on player
            Hue = reader.ReadUInt16();
            Font = reader.ReadInt16();
            CliLocNumber = reader.ReadInt32();
            SpeakerName = reader.ReadString(30);
            Arguements = reader.ReadUnicodeStringSafeReverse();
            // what about the arguments?
            // http://docs.polserver.com/packets/index.php?Packet=0xC1
        }
    }
}
