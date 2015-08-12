using System;
using System.IO;

namespace UltimaXNA.Ultima.Resources
{
    public struct EffectData
    {
        private readonly byte m_Unknown;

        public readonly sbyte[] Frames;
        public readonly byte FrameCount;
        public readonly byte FrameInterval;
        public readonly byte StartInterval;

        public EffectData(BinaryReader reader)
        {
            byte[] data = reader.ReadBytes(0x40);
            Frames = Array.ConvertAll(data, b => unchecked((sbyte)b));
            m_Unknown = reader.ReadByte();
            FrameCount = reader.ReadByte();
            if (FrameCount == 0)
            {
                FrameCount = 1;
                Frames[0] = 0;
            }
            FrameInterval = reader.ReadByte();
            if (FrameInterval == 0)
                FrameInterval = 1;
            StartInterval = reader.ReadByte();
        }
    }
}
