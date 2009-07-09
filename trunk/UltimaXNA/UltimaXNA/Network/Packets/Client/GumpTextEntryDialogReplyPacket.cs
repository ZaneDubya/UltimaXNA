using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class GumpTextEntryDialogReplyPacket : SendPacket
    {
        public GumpTextEntryDialogReplyPacket(int id, byte type, byte index, string reply)
            : base(0xAC, "Gump TextEntry Dialog Reply")
        {
            Stream.Write(id);
            Stream.Write((byte)type);
            Stream.Write((byte)index);
            Stream.Write((short)0x0000);
            Stream.Write((byte)0x00);
            Stream.WriteAsciiNull(reply);
        }
    }
}
