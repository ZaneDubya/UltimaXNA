/***************************************************************************
 *   EffectData.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
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
