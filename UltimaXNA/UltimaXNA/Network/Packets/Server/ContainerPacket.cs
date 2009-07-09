using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class OpenContainerPacket : RecvPacket
    {
        readonly Serial _serial;
        readonly ushort _gumpId;

        public Serial Serial { get { return _serial; } }
        public ushort GumpId { get { return _gumpId; } }

        public OpenContainerPacket(PacketReader reader)
            : base(0x24, "Open Container")
        {
            this._serial = reader.ReadInt32();
            this._gumpId = reader.ReadUInt16();
        }
    }
}
