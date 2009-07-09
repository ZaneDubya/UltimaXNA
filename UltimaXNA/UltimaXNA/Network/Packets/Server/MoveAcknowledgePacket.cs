using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class MoveAcknowledgePacket : RecvPacket
    {
        readonly byte _sequence;
        readonly byte _notoriety;

        public byte Sequence 
        {
            get { return _sequence; } 
        }

        public byte Notoriety
        {
            get { return _notoriety; }
        }

        public MoveAcknowledgePacket(PacketReader reader)
            : base(0x22, "Move Request Acknowledged")
        {
            _sequence = reader.ReadByte(); // (matches sent sequence)
            _notoriety = reader.ReadByte(); // Not sure why it sends this.
        }
    }
}
