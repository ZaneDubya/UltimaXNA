using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{

    public class WornItemPacket : RecvPacket
    {
        readonly Serial _serial;
        readonly short _itemId;
        readonly byte _layer;
        readonly Serial _parentSerial;
        readonly short _hue;

        public Serial Serial
        {
            get { return _serial; }
        }

        public short ItemId
        {
            get { return _itemId; }
        }

        public byte Layer
        {
            get { return _layer; }
        }

        public Serial ParentSerial
        {
            get { return _parentSerial; }
        }

        public short Hue
        {
            get { return _hue; }
        }


        public WornItemPacket(PacketReader reader)
            : base(0x2E, "Worn Item")
        {
            _serial = reader.ReadInt32();
            _itemId = reader.ReadInt16();
            reader.ReadByte();
            _layer = reader.ReadByte();
            _parentSerial = reader.ReadInt32();
            _hue = reader.ReadInt16();
        }
    }
}
