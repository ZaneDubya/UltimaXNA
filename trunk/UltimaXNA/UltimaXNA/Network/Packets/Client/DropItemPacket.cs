using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class DropItemPacket : SendPacket
    {
        public DropItemPacket(Serial serial, short x, short y, byte z, byte gridIndex, Serial containerSerial)
            : base(0x08, "Drop Item", 14)
        {
            Stream.Write(serial);
            Stream.Write((short)x);
            Stream.Write((short)y);
            Stream.Write((byte)z);
            Stream.Write((byte)gridIndex);
            Stream.Write(containerSerial);
        }
    }
}
