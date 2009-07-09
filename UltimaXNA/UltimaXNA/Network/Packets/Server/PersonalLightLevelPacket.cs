using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class PersonalLightLevelPacket : RecvPacket
    {
        readonly Serial _creatureSerial;
        readonly byte _lightLevel;

        public Serial CreatureSerial
        {
            get { return _creatureSerial; }
        }

        public byte LightLevel
        {
            get { return _lightLevel; }
        }

        public PersonalLightLevelPacket(PacketReader reader)
            : base(0x4E, "Personal Light Level")
        {
            this._creatureSerial = reader.ReadInt32();
            this._lightLevel = reader.ReadByte();
        }
    }
}
