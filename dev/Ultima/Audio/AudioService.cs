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
#region usings
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA.Ultima.Audio
{
    public class AudioService
    {
        private Dictionary<int, UOSound> m_Sounds = new Dictionary<int, UOSound>();
        private Dictionary<int, UOMusic> m_Music = new Dictionary<int, UOMusic>();
        private UOMusic m_MusicCurrentlyPlaying = null;

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
                if (SoundData.TryGetSoundData(soundIndex, out data, out name))
                {
                    sound.Name = name;
                    sound.WaveBuffer = data;
                    sound.Status = SoundState.Loaded;
                    sound.Play();
                }
            }
        }

        public void PlayMusic(int id)
        {
            if (id < 0) // not a valid id, used to stop music.
            {
                StopMusic();
                return;
            }

            if (!m_Music.ContainsKey(id))
            {
                string name;
                bool loops;
                if (UltimaXNA.Ultima.IO.MusicData.TryGetMusicData(id, out name, out loops))
                {
                    m_Music.Add(id, new UOMusic(id, name, loops));
                }
                else
                {
                    Tracer.Error("Received unknown music id {0}", id);
                    return;
                }
            }

            UOMusic toPlay = m_Music[id];

            if (toPlay != m_MusicCurrentlyPlaying)
            {
                // stop the current song
                StopMusic();

                m_MusicCurrentlyPlaying = toPlay;
                if (m_MusicCurrentlyPlaying.Status != SoundState.Loaded)
                    m_MusicCurrentlyPlaying.Load(); // this should really be threaded
                MediaPlayer.Play(m_MusicCurrentlyPlaying.Song);
            }
        }

        public void StopMusic()
        {
            if (m_MusicCurrentlyPlaying != null)
            {
                MediaPlayer.Stop();
                m_MusicCurrentlyPlaying = null;
            }
        }
    }
}
