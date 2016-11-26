/***************************************************************************
 *   UOSound.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using UltimaXNA.Core.Audio;

namespace UltimaXNA.Ultima.Audio
{
    class UOSound : ASound
    {
        private readonly byte[] m_WaveBuffer;

        public UOSound(string name, byte[] buffer)
            : base(name)
        {
            m_WaveBuffer = buffer;
        }

        protected override void OnBufferNeeded(object sender, EventArgs e)
        {
            // not needed.
        }

        protected override byte[] GetBuffer()
        {
            return m_WaveBuffer;
        }
    };
}
