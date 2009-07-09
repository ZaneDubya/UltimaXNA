using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class AsciiSpeechtPacket : SendPacket
    {
        public AsciiSpeechtPacket(short color, short font, string lang, string text)
            : base(0xAD, "Ascii Speech")
        {
            Stream.Write((byte)0);
            Stream.Write((short)color);
            Stream.Write((short)font);
            Stream.WriteAsciiNull(lang);
            Stream.Write((byte)0);
            Stream.Write((byte)0x10);
            Stream.Write((byte)0x02);
            Stream.WriteAsciiNull(text);
        }
    }
}
