using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class WarModePacket : RecvPacket
    {
        readonly byte _warmode;

        public bool WarMode
        {
            get { return (_warmode == 0x01); }
        }

        public WarModePacket(PacketReader reader)
            : base(0x72, "Request War Mode")
        {
            _warmode = reader.ReadByte();

            reader.ReadByte(); // always 0x00
            reader.ReadByte(); // always 0x32
            reader.ReadByte(); // always 0x00
        }
    }
}
