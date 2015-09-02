/***************************************************************************
 *   ASound.cs
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
using System.Collections.Generic;

namespace UltimaXNA.Core.Audio
{
    abstract class ASound : IDisposable
    {
        public string Name { get; private set; }

        abstract protected byte[] GetBuffer();
        abstract protected void OnBufferNeeded(object sender, EventArgs e);
        virtual protected void AfterStop() { }
        virtual protected void BeforePlay() { }

        private static List<Tuple<DynamicSoundEffectInstance, double>> m_EffectInstances;
        private static List<Tuple<DynamicSoundEffectInstance, double>> m_MusicInstances;
        protected DynamicSoundEffectInstance m_ThisInstance;

        protected int Frequency = 22050;
        protected AudioChannels Channels = AudioChannels.Mono;

        static ASound()
        {
            m_EffectInstances = new List<Tuple<DynamicSoundEffectInstance, double>>();
            m_MusicInstances = new List<Tuple<DynamicSoundEffectInstance, double>>();
        }

        public ASound(string name)
        {
            Name = name;
            
        }

        public void Dispose()
        {
            if (m_ThisInstance != null)
            {
                if (!m_ThisInstance.IsDisposed)
                {
                    m_ThisInstance.Stop();
                    m_ThisInstance.Dispose();
                }
                m_ThisInstance = null;
            }
        }

        /// <summary>
        /// Plays the effect.
        /// </summary>
        /// <param name="asEffect">Set to false for music, true for sound effects.</param>
        public void Play(bool asEffect, AudioEffects effect = AudioEffects.None, float volume = 1.0f)
        {
            double now = UltimaGame.TotalMS;
            CullExpiredEffects(now);

            m_ThisInstance = GetNewInstance(asEffect);
            if (m_ThisInstance == null)
            {
                this.Dispose();
                return;
            }

            switch (effect)
            {
                case AudioEffects.PitchVariation:
                    float pitch = (float)Utility.RandomValue(-5, 5) * .025f;
                    m_ThisInstance.Pitch = pitch;
                    break;
            }

            BeforePlay();

            byte[] buffer = GetBuffer();
            if (buffer != null && buffer.Length > 0)
            {
                m_ThisInstance.BufferNeeded += new EventHandler<EventArgs>(OnBufferNeeded);
                m_ThisInstance.SubmitBuffer(buffer);
                m_ThisInstance.Volume = volume;
                m_ThisInstance.Play();

                List<Tuple<DynamicSoundEffectInstance, double>> list = (asEffect) ? m_EffectInstances : m_MusicInstances;
                double ms = m_ThisInstance.GetSampleDuration(buffer.Length).TotalMilliseconds;
                list.Add(new Tuple<DynamicSoundEffectInstance, double>(m_ThisInstance, now + ms));
            }
        }

        public void Stop()
        {
            AfterStop();
        }

        private void CullExpiredEffects(double now )
        {
            // Check to see if any existing instances have stopped playing. If they have, remove the
            // reference to them so the garbage collector can collect them.
            for (int i = 0; i < m_EffectInstances.Count; i++)
            {
                if (m_EffectInstances[i].Item1.IsDisposed || m_EffectInstances[i].Item1.State == SoundState.Stopped || m_EffectInstances[i].Item2 <= now)
                {
                    m_EffectInstances[i].Item1.Dispose();
                    m_EffectInstances.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < m_MusicInstances.Count; i++)
            {
                if (m_MusicInstances[i].Item1.IsDisposed || m_MusicInstances[i].Item1.State == SoundState.Stopped)
                {
                    m_MusicInstances[i].Item1.Dispose();
                    m_MusicInstances.RemoveAt(i);
                    i--;
                }
            }
        }

        private DynamicSoundEffectInstance GetNewInstance(bool asEffect)
        {
            List<Tuple<DynamicSoundEffectInstance, double>> list = (asEffect) ? m_EffectInstances : m_MusicInstances;
            int maxInstances = (asEffect) ? 32 : 2;
            if (list.Count >= maxInstances)
                return null;
            else
                return new DynamicSoundEffectInstance(Frequency, Channels); // shouldn't we be recycling these?
        }
    }
}
