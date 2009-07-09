using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class TargetCursorPacket : RecvPacket
    {
        readonly byte _commandtype;
        readonly int _cursorid;
        readonly byte _cursortype;

        public byte CommandType 
        {
            get { return _commandtype; }
        }

        public int CursorID 
        {
            get { return _cursorid; }         
        }

        public byte CursorType 
        {
            get { return _cursortype; } 
        }
        
        public TargetCursorPacket(PacketReader reader)
            : base(0x6C, "Target Cursor")
        {
            _commandtype = reader.ReadByte(); // 0x00 = Select Object; 0x01 = Select X, Y, Z
            _cursorid = reader.ReadInt32();
            _cursortype = reader.ReadByte(); // 0 - 2 = unknown; 3 = Cancel current targetting RunUO seems to always send 0.
        }
    }
}
