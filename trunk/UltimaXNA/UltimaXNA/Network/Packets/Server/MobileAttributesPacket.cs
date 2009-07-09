using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class MobileAttributesPacket : RecvPacket
    {
        readonly Serial _serial;
        readonly short _maxHits;
        readonly short _currentHits;
        readonly short _maxMana;
        readonly short _currentMana;
        readonly short _maxStamina;
        readonly short _currentStamina;

        public Serial Serial
        {
            get { return _serial; }
        }

        public short MaxHits
        {
            get { return _maxHits; }
        }

        public short CurrentHits
        {
            get { return _currentHits; }
        }

        public short MaxMana
        {
            get { return _maxMana; }
        }

        public short CurrentMana
        {
            get { return _currentMana; }
        }

        public short MaxStamina
        {
            get { return _maxStamina; }
        }

        public short CurrentStamina
        {
            get { return _currentStamina; }
        }


        public MobileAttributesPacket(PacketReader reader)
            : base(0x2D, "Mobile Attributes")
        {
            _serial = reader.ReadInt32();
            _maxHits = reader.ReadInt16();
            _currentHits = reader.ReadInt16();
            _maxMana = reader.ReadInt16();
            _currentMana = reader.ReadInt16();
            _maxStamina = reader.ReadInt16();
            _currentStamina = reader.ReadInt16();
        }
    }
}
