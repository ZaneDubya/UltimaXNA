using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class PlaySoundEffectPacket : RecvPacket
    {
        public readonly int Mode;
        public readonly int SoundModel;
        public readonly int Unknown;
        public readonly int X, Y, Z;
        public PlaySoundEffectPacket(PacketReader reader)
            : base(0x54, "Play Sound Effect")
        {
            Mode = reader.ReadByte();
            SoundModel = reader.ReadInt16();
            Unknown = reader.ReadInt16();
            X = reader.ReadInt16();
            Y = reader.ReadInt16();
            Z = reader.ReadInt16();
        }
    }
}
