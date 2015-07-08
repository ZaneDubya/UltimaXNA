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
#region Usings
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.Configuration;
#endregion

namespace UltimaXNA.Configuration
{
    public class AudioSettings : ASettingsSection
    {
        public const string SectionName = "audio";

        private int m_MusicVolume;
        private int m_SoundVolume;
        private bool m_MusicOn;
        private bool m_SoundOn;

        public AudioSettings()
        {
            MusicVolume = 100;
            SoundVolume = 100;

            MusicOn = false;
            SoundOn = true;
        }

        public int MusicVolume
        {
            get { return m_MusicVolume; }
            set { SetProperty(ref m_MusicVolume, value); }
        }

        public int SoundVolume
        {
            get { return m_SoundVolume; }
            set { SetProperty(ref m_SoundVolume, value); }
        }

        public bool MusicOn
        {
            get { return m_MusicOn; }
            set { SetProperty(ref m_MusicOn, value); }
        }

        public bool SoundOn
        {
            get { return m_SoundOn; }
            set { SetProperty(ref m_SoundOn, value); }
        }
    }
}
