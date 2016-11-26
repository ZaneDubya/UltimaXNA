/***************************************************************************
 *   AudioSettings.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Configuration;
#endregion

namespace UltimaXNA.Configuration
{
    public class AudioSettings : ASettingsSection
    {
        int m_MusicVolume;
        int m_SoundVolume;
        bool m_MusicOn;
        bool m_SoundOn;
        bool m_FootStepSoundOn;

        public AudioSettings()
        {
            MusicVolume = 100;
            SoundVolume = 100;

            MusicOn = true;
            SoundOn = true;
            FootStepSoundOn = true;
        }

        /// <summary>
        /// Volume of music. Value is percent of max volume, clamped to 0 - 100.
        /// </summary>
        public int MusicVolume
        {
            get { return m_MusicVolume; }
            set{ SetProperty(ref m_MusicVolume, Clamp(value, 0, 100)); }
        }

        /// <summary>
        /// Volume of sound effects. Value is percent of max volume, clamped to 0 - 100.
        /// </summary>
        public int SoundVolume
        {
            get { return m_SoundVolume; }
            set { SetProperty(ref m_SoundVolume, Clamp(value, 0, 100)); }
        }

        /// <summary>
        /// False = requests to play music are ignored.
        /// </summary>
        public bool MusicOn
        {
            get { return m_MusicOn; }
            set { SetProperty(ref m_MusicOn, value); }
        }

        /// <summary>
        /// False = requests to play sound effects are ignored.
        /// </summary>
        public bool SoundOn
        {
            get { return m_SoundOn; }
            set { SetProperty(ref m_SoundOn, value); }
        }

        /// <summary>
        /// False = no foot step sound effects ever play.
        /// </summary>
        public bool FootStepSoundOn
        {
            get { return m_FootStepSoundOn; }
            set { SetProperty(ref m_FootStepSoundOn, value); }
        }
    }
}
