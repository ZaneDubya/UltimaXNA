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
using System;
using UltimaXNA.Core.Audio;
using UltimaXNA.Core.Audio.MP3Sharp;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Ultima.IO;

namespace UltimaXNA.Ultima.Audio
{
    class UOMusic : ASound
    {
        MP3Stream m_Stream;
        const int NUMBER_OF_PCM_BYTES_TO_READ_PER_CHUNK = 0x8000; // 32768 bytes, about 0.9 seconds
        readonly byte[] m_WaveBuffer = new byte[NUMBER_OF_PCM_BYTES_TO_READ_PER_CHUNK];
        bool m_Repeat;
        bool m_Playing;

        protected string Path => FileManager.GetPath(string.Format("Music/Digital/{0}.mp3", Name));
        public UOMusic(int index, string name, bool loop)
            : base(name)
        {
            m_Repeat = loop;
            m_Playing = false;
            Channels = AudioChannels.Stereo;
        }

        public void Update()
        {
            // sanity - if the buffer empties, we will lose our sound effect. Thus we must continually check if it is dead.
            OnBufferNeeded(null, null);
        }

        protected override byte[] GetBuffer()
        {
            if (m_Playing)
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
            Stop();
            return null;
        }

        protected override void OnBufferNeeded(object sender, EventArgs e)
        {
            if (m_Playing)
            {
                while (m_ThisInstance.PendingBufferCount < 3)
                {
                    byte[] buffer = GetBuffer();
                    if (m_ThisInstance.IsDisposed)
                        return;
                    m_ThisInstance.SubmitBuffer(buffer);
                }
            }
        }

        protected override void BeforePlay()
        {
            if (m_Playing)
            {
                Stop();
            }

            try
            {
                m_Stream = new MP3Stream(Path, NUMBER_OF_PCM_BYTES_TO_READ_PER_CHUNK);
                Frequency = m_Stream.Frequency;

                m_Playing = true;
            }
            catch (Exception e)
            {
                // file in use or access denied.
                Tracer.Error(e);
                m_Playing = false;
            }
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