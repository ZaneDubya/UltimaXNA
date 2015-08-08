/***************************************************************************
 *   UOMusic.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using Microsoft.Xna.Framework.Audio;
using UltimaXNA.Core.Audio;
using UltimaXNA.Core.Audio.MP3Sharp;
using UltimaXNA.Ultima.IO;

namespace UltimaXNA.Ultima.Audio
{
    class UOMusic : ASound
    {
        private MP3Stream m_Stream;
        private const int NUMBER_OF_PCM_BYTES_TO_READ_PER_CHUNK = 8192;
        private readonly byte[] m_WaveBuffer = new byte[NUMBER_OF_PCM_BYTES_TO_READ_PER_CHUNK];

        private bool m_Repeat;
        private bool m_Playing;

        protected string Path
        {
            get
            {
                string path = FileManager.GetPath(string.Format("Music\\Digital\\{0}.mp3", Name));
                return path;
            }
        }

        public UOMusic(int index, string name, bool loop)
            : base(name)
        {
            m_Repeat = loop;
            m_Playing = false;
        }

        protected override byte[] GetBuffer()
        {
            int bytesReturned = m_Stream.Read(m_WaveBuffer, 0, m_WaveBuffer.Length);
            if (bytesReturned != NUMBER_OF_PCM_BYTES_TO_READ_PER_CHUNK)
            {
                if (m_Repeat)
                {
                    m_Stream.Position = 0;
                    m_Stream.Read(m_WaveBuffer, bytesReturned, m_WaveBuffer.Length - bytesReturned);
                }
                else
                {
                    if (bytesReturned == 0)
                    {
                        Stop();
                    }
                }
            }
            return m_WaveBuffer;
        }

        protected override void OnBufferNeeded(object sender, System.EventArgs e)
        {
            if (m_Playing)
            {
                DynamicSoundEffectInstance instance = sender as DynamicSoundEffectInstance;
                while (instance.PendingBufferCount < 3)
                {
                    byte[] buffer = GetBuffer();
                    if (instance.IsDisposed)
                        return;
                    instance.SubmitBuffer(buffer);
                }
            }
        }

        protected override void BeforePlay()
        {
            if (m_Playing)
            {
                Stop();
            }

            m_Stream = new MP3Stream(Path, NUMBER_OF_PCM_BYTES_TO_READ_PER_CHUNK);
            Frequency = m_Stream.Frequency;
            Channels = AudioChannels.Stereo;

            m_Playing = true;
        }

        protected override void AfterStop()
        {
            if (m_Playing)
            {
                m_Playing = false;

                m_Stream.Close();
                m_Stream = null;
            }
        }
    }
}
