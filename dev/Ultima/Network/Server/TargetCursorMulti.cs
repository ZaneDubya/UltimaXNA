#region usings
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    class TargetCursorMultiPacket : SendRecvPacket
    {
        public readonly Serial DeedSerial;
        public readonly int MultiModel;
        public readonly byte CursorType;
        public readonly int OffsetX;
        public readonly int OffsetY;
        public readonly int OffsetZ;

        // This is called when the packet is received.
        public TargetCursorMultiPacket(PacketReader reader)
            : base(0x99, "Target Cursor For Multi")
        {
            reader.ReadByte(); // (0x01 from server, 0x00 from client) 
            DeedSerial = reader.ReadInt32();
            CursorType = reader.ReadByte(); // flag byte. Harmful = 1, Beneficial = 2. Unused.
            reader.ReadBytes(11); // unknown (all 0)
            MultiModel = reader.ReadInt16();
            OffsetX = reader.ReadInt16();
            OffsetY = reader.ReadInt16();
            OffsetZ = reader.ReadInt16();
        }
    }
}
