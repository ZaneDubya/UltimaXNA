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
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace UltimaXNA.Ultima.Audio
{
    class UOSound
    {
        public string Name;
        public byte[] WaveBuffer;
        public SoundState Status = SoundState.Unloaded;

        private readonly List<Tuple<DynamicSoundEffectInstance, float>> m_Instances;

        public UOSound()
        {
            m_Instances = new List<Tuple<DynamicSoundEffectInstance, float>>();
        }

        public void Play()
        {
            float now = (float)UltimaGame.TotalMS;

            // Check to see if any existing instances of this sound effect have stopped playing. If
            // they have, remove the reference to them so the garbage collector can collect them.
            for (int i = 0; i < m_Instances.Count; i++)
                if (m_Instances[i].Item2 < now)
                {
                    m_Instances.RemoveAt(i);
                    i--;
                }

            DynamicSoundEffectInstance instance = new DynamicSoundEffectInstance(22050, AudioChannels.Mono);
            instance.BufferNeeded += new EventHandler<EventArgs>(instance_BufferNeeded);
            instance.SubmitBuffer(WaveBuffer);
            instance.Play();
            m_Instances.Add(new Tuple<DynamicSoundEffectInstance, float>(instance,
                now + (instance.GetSampleDuration(WaveBuffer.Length).Milliseconds)));
        }

        void instance_BufferNeeded(object sender, EventArgs e)
        {
            // do nothing.
        }
    };
}
