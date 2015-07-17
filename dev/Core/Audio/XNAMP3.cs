// /***************************************************************************
//  * XNAMP3.cs
//  * Copyright (c) 2015 the authors.
//  * 
//  * All rights reserved. This program and the accompanying materials
//  * are made available under the terms of the GNU Lesser General Public License
//  * (LGPL) version 3 which accompanies this distribution, and is available at
//  * https://www.gnu.org/licenses/lgpl-3.0.en.html
//  *
//  * This library is distributed in the hope that it will be useful,
//  * but WITHOUT ANY WARRANTY; without even the implied warranty of
//  * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  * Lesser General Public License for more details.
//  *
//  ***************************************************************************/

using System;
using Microsoft.Xna.Framework.Audio;
using UltimaXNA.Core.Audio.MP3Sharp;

namespace UltimaXNA.Core.Audio
{
    class XNAMP3 : IDisposable
    {
        private MP3Stream m_Stream;
        private DynamicSoundEffectInstance m_Instance;

        private const int NUMBER_OF_PCM_BYTES_TO_READ_PER_CHUNK = 4096;
        private readonly byte[] m_WaveBuffer = new byte[NUMBER_OF_PCM_BYTES_TO_READ_PER_CHUNK];

        private bool m_Repeat;
        private bool m_Playing;

        public XNAMP3(string path)
        {
            m_Stream = new MP3Stream(path, NUMBER_OF_PCM_BYTES_TO_READ_PER_CHUNK);
            m_Instance = new DynamicSoundEffectInstance(m_Stream.Frequency, AudioChannels.Stereo);
        }

        public void Dispose()
        {
            if (m_Playing)
            {
                Stop();
            }

            m_Instance.Dispose();
            m_Instance = null;

            m_Stream.Close();
            m_Stream = null;
        }

        public void Play(bool repeat = false)
        {
            if (m_Playing)
            {
                Stop();
            }

            m_Playing = true;
            m_Repeat = repeat;
            
            SubmitBuffer(3);
            m_Instance.BufferNeeded += instance_BufferNeeded;
            m_Instance.Play();
        }

        public void Stop()
        {
            if (m_Playing)
            {
                m_Playing = false;

                m_Instance.Stop();
                m_Instance.BufferNeeded -= instance_BufferNeeded;
            }
        }

        private void instance_BufferNeeded(object sender, EventArgs e)
        {
            SubmitBuffer();
        }

        private void SubmitBuffer(int count = 1)
        {
            while (count > 0)
            {
                ReadFromStream();
                m_Instance.SubmitBuffer(m_WaveBuffer);
                count--;
            }
        }

        private void ReadFromStream()
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
        }
    }
}
