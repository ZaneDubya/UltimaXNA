using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class DragEffectPacket : RecvPacket
    {
        readonly int _itemId;
        readonly int _amount;
        readonly Serial _sourceContainer;
        readonly int _sourceX;
        readonly int _sourceY;
        readonly int _sourceZ;
        readonly Serial _destContainer;
        readonly int _destX;
        readonly int _destY;
        readonly int _destZ;

        public int ItemId 
        {
            get { return _itemId; }
        }

        public int Amount 
        {
            get { return _itemId; } 
        }

        public Serial SourceContainer 
        {
            get { return _sourceContainer; }
        }

        public int SourceX 
        {
            get { return _sourceX; } 
        }

        public int SourceY 
        {
            get { return _sourceY; } 
        }

        public int SourceZ
        {
            get { return _sourceZ; } 
        }

        public Serial DestContainer 
        {
            get { return _destContainer; } 
        }

        public int DestX 
        {
            get { return _destX; } 
        }

        public int DestY         
        {
            get { return _destY; }
        }

        public int DestZ 
        {
            get { return _destZ; }
        }

        public DragEffectPacket(PacketReader reader)
            : base(0x23, "Dragging Item")
        {
            _itemId = reader.ReadUInt16();
            reader.ReadByte(); // 0x03 bytes unknown.
            reader.ReadByte(); //
            reader.ReadByte(); //
            _amount = reader.ReadUInt16();
            _sourceContainer = reader.ReadInt32(); // 0xFFFFFFFF for ground
            _sourceX = reader.ReadUInt16();
            _sourceY = reader.ReadUInt16();
            _sourceZ = reader.ReadByte();
            _destContainer = reader.ReadInt32(); // 0xFFFFFFFF for ground
            _destX = reader.ReadUInt16();
            _destY = reader.ReadUInt16();
            _destZ = reader.ReadByte();
        }
    }
}
