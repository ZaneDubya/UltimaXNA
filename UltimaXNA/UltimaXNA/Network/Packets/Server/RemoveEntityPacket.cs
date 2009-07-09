using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class RemoveEntityPacket : RecvPacket
    {
        readonly Serial _serial;

        public Serial Serial
        {
            get { return _serial; }
        }

        public RemoveEntityPacket(PacketReader reader)
            : base(0x1D, "Remove Entity")
        {
            _serial = reader.ReadInt32();
        }
    }
}
