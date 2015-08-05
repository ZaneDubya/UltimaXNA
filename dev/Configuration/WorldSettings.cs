/***************************************************************************
 *   WorldSettings.cs
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
using UltimaXNA.Core.Input.Windows;
#endregion

namespace UltimaXNA.Configuration
{
    public class WorldSettings : ASettingsSection
    {
        public const string SectionName = "world";

        private ResolutionConfig m_FullScreenResolution;
        private ResolutionConfig m_WindowResolution;
        private ResolutionConfig m_WorldGumpResolution;
        private bool m_IsFullScreen;
        private MouseConfig m_Mouse;
        private bool m_AlwaysRun;
        private bool m_MenuBarDisabled;

        public WorldSettings()
        {
            FullScreenResolution = new ResolutionConfig(1024, 768);
            WindowResolution = new ResolutionConfig(1024, 768);
            PlayWindowGumpResolution = new ResolutionConfig(1024, 768);
            IsMaximized = false;
            Mouse = new MouseConfig();
            AlwaysRun = false;
            MenuBarDisabled = false;
        }

        public bool IsMaximized
        {
            get { return m_IsFullScreen; }
            set { SetProperty(ref m_IsFullScreen, value); }
        }

        public MouseConfig Mouse
        {
            get { return m_Mouse; }
            set { SetProperty(ref m_Mouse, value); }
        }

        public ResolutionConfig FullScreenResolution
        {
            get { return m_FullScreenResolution; }
            set { SetProperty(ref m_FullScreenResolution, value); }
        }

        public ResolutionConfig WindowResolution
        {
            get { return m_WindowResolution; }
            set { SetProperty(ref m_WindowResolution, value); }
        }

        public ResolutionConfig PlayWindowGumpResolution
        {
            get { return m_WorldGumpResolution; }
            set { SetProperty(ref m_WorldGumpResolution, value); }
        }

        public bool AlwaysRun
        {
            get { return m_AlwaysRun; }
            set { SetProperty(ref m_AlwaysRun, value); }
        }

        public bool MenuBarDisabled
        {
            get { return m_MenuBarDisabled; }
            set { SetProperty(ref m_MenuBarDisabled, value); }
        }
    }
}
