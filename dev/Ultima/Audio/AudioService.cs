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
using System;
using System.Collections.Generic;
using UltimaXNA.Core.Audio;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA.Ultima.Audio
{
    public class AudioService
    {
        private readonly Dictionary<int, UOSound> m_Sounds = new Dictionary<int, UOSound>();
        private readonly Dictionary<int, UOMusic> m_Music = new Dictionary<int, UOMusic>();

        private UOMusic m_MusicCurrentlyPlaying = null;
        private XNAMP3 m_MusicCurrentlyPlayingMP3 = null;

        public void PlaySound(int soundIndex)
        {
            if (Settings.Audio.SoundOn)
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
        }

        public void PlayMusic(int id)
        {
            if (Settings.Audio.MusicOn)
            {
                if (id < 0) // not a valid id, used to stop music.
                {
                    StopMusic();
                    Tracer.Error("Received unknown music id {0}", id);
                    return;
                }

                if (!m_Music.ContainsKey(id))
                {
                    string name;
                    bool loops;
                    if (MusicData.TryGetMusicData(id, out name, out loops))
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

                    // if (m_MusicCurrentlyPlaying.Status != SoundState.Loaded)
                    //    m_MusicCurrentlyPlaying.Load(); // this should really be threaded

                    // open resource
                    string mciCommand = string.Format("open \"{0}\" type MPEGVideo alias {1}", toPlay.Path, c_InternalMusicName);
                    int result = SendMediaPlayerCommand(mciCommand, null, 0, IntPtr.Zero);
                    if (result == 0)
                    {
                        m_MusicCurrentlyPlaying = toPlay;
                        // start playing
                        string playCommand = string.Format("play {0} from 0", c_InternalMusicName);
                        if (m_MusicCurrentlyPlaying.DoLoop)
                        {
                            playCommand += " repeat";
                        }
                        if (SendMediaPlayerCommand(playCommand, null, 0, IntPtr.Zero) != 0)
                        {
                            Tracer.Error("Error playing mp3 file {0}", toPlay.Path);
                        }
                    }
                    else
                    {
                        Tracer.Error("Error opening mp3 file {0}", toPlay.Path);
                    }
                }
            }
        }

        public void StopMusic()
        {
            if (m_MusicCurrentlyPlaying != null)
            {
                m_MusicCurrentlyPlayingMP3.Stop();
                m_MusicCurrentlyPlayingMP3.Dispose();
                m_MusicCurrentlyPlaying = null;
            }
        }
    }
}
