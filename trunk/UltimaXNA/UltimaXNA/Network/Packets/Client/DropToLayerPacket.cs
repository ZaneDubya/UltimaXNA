using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class DropToLayerPacket : SendPacket
    {
        public DropToLayerPacket(Serial itemSerial, byte layer, Serial playerSerial)
            : base(0x13, "Drop To Layer", 10)
        {
            Stream.Write(itemSerial);
            Stream.Write((byte)layer);
            Stream.Write(playerSerial);
        }
    }
}
