using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class OverallLightLevelPacket : RecvPacket
    {
        readonly byte _lightLevel;

        public byte LightLevel
        {
            get { return _lightLevel; }
        }

        public OverallLightLevelPacket(PacketReader reader)
            : base(0x4F, "OverallLightLevel")
        {
            this._lightLevel = reader.ReadByte();
        }
    }
}
