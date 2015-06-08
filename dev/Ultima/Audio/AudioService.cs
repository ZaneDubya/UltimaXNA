/***************************************************************************
 *   AudioService.cs
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
using System.Linq;
using System.Text;
using UltimaXNA.Ultima.IO;

namespace UltimaXNA.Ultima.Audio
{
    public class AudioService
    {
        private static Dictionary<int, UOSound> m_Sounds = new Dictionary<int, UOSound>();

        public void PlaySound(int soundIndex)
        {
            UOSound sound;
            if (m_Sounds.TryGetValue(soundIndex, out sound))
            {
                if (sound.Status == SoundState.Loaded)
                    sound.Play();
            }
            else
            {
                sound = new UOSound();
                m_Sounds.Add(soundIndex, sound);
                string name;
                byte[] data;
                if (SoundData.TryGetSoundData(soundIndex - 1, out data, out name))
                {
                    sound.Name = name;
                    sound.WaveBuffer = data;
                    sound.Status = SoundState.Loaded;
                    sound.Play();
                }
            }


        }
    }
}
