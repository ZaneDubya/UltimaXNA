#region usings
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.UltimaPackets.Server
{
    class TargetCursorMultiPacket : SendRecvPacket
    {
        readonly Serial _deedSerial;
        readonly int _multiModel;
        readonly int _offsetX, _offsetY, _offsetZ;

        public Serial DeedSerial 
        {
            get { return _deedSerial; }
        }

        public int MultiModel 
        {
            get { return _multiModel; }         
        }

        public int OffsetX { get { return _offsetX; } }
        public int OffsetY { get { return _offsetY; } }
        public int OffsetZ { get { return _offsetZ; } }

        // This is called when the packet is received.
        public TargetCursorMultiPacket(PacketReader reader)
            : base(0x99, "Target Cursor For Multi")
        {
            reader.ReadByte(); // (0x01 from server, 0x00 from client) 
            _deedSerial = reader.ReadInt32();
            reader.ReadByte(); // flag byte. Harmful = 1, Beneficial = 2. Unused.
            reader.ReadBytes(11); // unknown (all 0)
            _multiModel = reader.ReadInt16();
            _offsetX = reader.ReadInt16();
            _offsetY = reader.ReadInt16();
            _offsetZ = reader.ReadInt16();
        }
    }
}
