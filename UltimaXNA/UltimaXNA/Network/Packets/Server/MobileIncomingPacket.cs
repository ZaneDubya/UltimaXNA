using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class MobileIncomingPacket : RecvPacket
    {
        readonly Serial _serial;
        readonly short _body;        
        readonly short _x;
        readonly short _y;
        readonly short _z;
        readonly byte _direction;
        readonly ushort _hue;
        public readonly MobileFlags Flags;
        readonly byte _notoriety;
        readonly EquipmentEntry[] _equipment;

        public Serial Serial
        {
            get { return _serial; } 
        }

        public short BodyID 
        {
            get { return _body; }
        }

        public short X
        {
            get { return _x; } 
        }

        public short Y 
        {
            get { return _y; } 
        }

        public short Z 
        {
            get { return _z; } 
        }

        public byte Direction
        {
            get { return _direction; } 
        }

        public ushort Hue 
        {
            get { return _hue; } 
        }

        public EquipmentEntry[] Equipment
        {
            get { return _equipment; }
        }

        
        

        /// <summary>
        /// 0x1: Innocent (Blue)
        /// 0x2: Friend (Green)
        /// 0x3: Grey (Grey - Non Criminal)
        /// 0x4: Criminal (Grey)
        /// 0x5: Enemy (Orange)
        /// 0x6: Murderer (Red)
        /// 0x7: Invulnerable (Yellow)
        /// </summary>
        public byte Notoriety
        {
            get { return _notoriety; }
        }  

        public MobileIncomingPacket(PacketReader reader)
            : base(0x78, "Mobile Incoming")
        {
            // Mobile
            _serial = reader.ReadInt32();
            _body = reader.ReadInt16();
            _x = reader.ReadInt16();
            _y = reader.ReadInt16();
            _z = reader.ReadSByte();
            _direction = reader.ReadByte();
            _hue = reader.ReadUInt16();
            this.Flags = new MobileFlags(reader.ReadByte());
            _notoriety = reader.ReadByte();

            // Read equipment - nine bytes ea.
            int equipCount = (reader.Size - reader.Index) / 9 ;
            _equipment = new EquipmentEntry[equipCount];

            for(int i = 0; i < equipCount; i++)
            {
                Serial serial = reader.ReadInt32();

                ushort gumpId = reader.ReadUInt16();
                byte layer = reader.ReadByte();
                ushort hue = 0;

                if ((gumpId & 0x8000) == 0x8000)
                {
                    gumpId = (ushort)((int)gumpId - 0x8000);
                    hue = reader.ReadUInt16();
                }

                _equipment[i] = new EquipmentEntry(serial, gumpId, layer, hue);
            }

            reader.ReadByte(); //zero terminated
        }
    }
}
