using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using System.Text;

namespace UltimaXNA.Core.Audio
{
    abstract class ASound : IDisposable
    {
        public string Name { get; private set; }

        private List<Tuple<DynamicSoundEffectInstance, double>> m_Instances;

        protected int Frequency = 22050;
        protected AudioChannels Channels = AudioChannels.Mono;

        public ASound(string name)
        {
            Name = name;
            m_Instances = new List<Tuple<DynamicSoundEffectInstance, double>>();
        }

        public void Dispose()
        {
            Stop();
        }

        public void Play()
        {
            BeforePlay();
            
            double now = UltimaGame.TotalMS;

            // Check to see if any existing instances of this sound effect have stopped playing. If
            // they have, remove the reference to them so the garbage collector can collect them.
            for (int i = 0; i < m_Instances.Count; i++)
            {
                if (m_Instances[i].Item2 < now)
                {
                    m_Instances[i].Item1.Dispose();
                    m_Instances.RemoveAt(i);
                    i--;
                }
            }

            byte[] buffer = GetBuffer();
            if (buffer != null && buffer.Length > 0)
            {
                DynamicSoundEffectInstance instance = new DynamicSoundEffectInstance(Frequency, Channels);
                instance.BufferNeeded += new EventHandler<EventArgs>(OnBufferNeeded);
                instance.SubmitBuffer(buffer);
                instance.Play();
                m_Instances.Add(new Tuple<DynamicSoundEffectInstance, double>(instance,
                    now + (instance.GetSampleDuration(buffer.Length).Milliseconds)));
            }
        }

        public void Stop()
        {
            while (m_Instances.Count > 0)
            {
                m_Instances[0].Item1.Stop();
                m_Instances[0].Item1.BufferNeeded -= OnBufferNeeded;
                m_Instances[0].Item1.Dispose();
                m_Instances.RemoveAt(0);
            }
            m_Instances.Clear();

            AfterStop();
        }

        abstract protected byte[] GetBuffer();

        abstract protected void OnBufferNeeded(object sender, EventArgs e);

        virtual protected void AfterStop() { }
        virtual protected void BeforePlay() { }
    }
}
