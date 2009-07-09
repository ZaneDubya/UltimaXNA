using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class ContainerContentUpdatePacket : RecvPacket
    {
        readonly Serial _serial;
        readonly int _itemId;
        readonly int _amount;
        readonly int _x;
        readonly int _y;
        readonly int _gridLocation;
        readonly int _containerSerial;
        readonly int _hue;

        public int ItemId
        {
            get { return _itemId; }
        }

        public int Amount
        {
            get { return _amount; }
        }

        public int X
        {
            get { return _x; }
        }

        public int Y
        {
            get { return _y; }
        }

        public int GridLocation
        {
            get { return _gridLocation; }
        }

        public int ContainerSerial
        {
            get { return _containerSerial; }
        }

        public int Hue
        {
            get { return _hue; }
        }

        public Serial Serial
        {
            get { return _serial; }
        }

        public ContainerContentUpdatePacket(PacketReader reader)
            : base(0x25, "Add Single Item")
        {
            _serial = reader.ReadInt32();
            _itemId = reader.ReadUInt16();
            reader.ReadByte(); // unknown 
            _amount = reader.ReadUInt16();
            _x = reader.ReadInt16();
            _y = reader.ReadInt16();
            _gridLocation = reader.ReadByte(); // always 0 in RunUO.
            _containerSerial = reader.ReadInt32();
            _hue = reader.ReadUInt16();
        }
    }
}
