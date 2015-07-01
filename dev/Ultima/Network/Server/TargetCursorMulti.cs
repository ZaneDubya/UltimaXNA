#region usings
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    class TargetCursorMultiPacket : SendRecvPacket
    {
        readonly Serial m_deedSerial;
        readonly int m_multiModel;
        readonly int m_offsetX, m_offsetY, m_offsetZ;

        public Serial DeedSerial 
        {
            get { return m_deedSerial; }
        }

        public int MultiModel 
        {
            get { return m_multiModel; }         
        }

        public int OffsetX { get { return m_offsetX; } }
        public int OffsetY { get { return m_offsetY; } }
        public int OffsetZ { get { return m_offsetZ; } }

        // This is called when the packet is received.
        public TargetCursorMultiPacket(PacketReader reader)
            : base(0x99, "Target Cursor For Multi")
        {
            reader.ReadByte(); // (0x01 from server, 0x00 from client) 
            m_deedSerial = reader.ReadInt32();
            reader.ReadByte(); // flag byte. Harmful = 1, Beneficial = 2. Unused.
            reader.ReadBytes(11); // unknown (all 0)
            m_multiModel = reader.ReadInt16();
            m_offsetX = reader.ReadInt16();
            m_offsetY = reader.ReadInt16();
            m_offsetZ = reader.ReadInt16();
        }
    }
}
