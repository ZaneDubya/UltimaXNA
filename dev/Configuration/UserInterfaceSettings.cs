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
using UltimaXNA.Core;
using UltimaXNA.Configuration.Properties;
using UltimaXNA.Ultima.Resources;
#endregion

namespace UltimaXNA.Configuration
{
    public class UserInterfaceSettings : ASettingsSection
    {
        ResolutionProperty m_FullScreenResolution;
        ResolutionProperty m_WindowResolution;
        ResolutionProperty m_WorldGumpResolution;
        bool m_PlayWindowPixelDoubling;
        bool m_IsFullScreen;
        MouseProperty m_Mouse;
        bool m_AlwaysRun;
        bool m_MenuBarDisabled;

        int m_SpeechColor = 4 + Utility.RandomValue(0, 99) * 5;
        int m_EmoteColor = 646;
        int m_PartyMsgPrivateColor = 58;
        int m_PartyMsgColor = 68;
        int m_GuildMsgColor = 70;
        bool m_IgnoreGuildMsg;
        int m_AllianceMsgColor = 487;
        bool m_IgnoreAllianceMsg;
        bool m_CrimeQuery;

        public UserInterfaceSettings()
        {
            FullScreenResolution = new ResolutionProperty();
            WindowResolution = new ResolutionProperty();
            PlayWindowGumpResolution = new ResolutionProperty();
            m_PlayWindowPixelDoubling = false;
            IsMaximized = false;
            Mouse = new MouseProperty();
            AlwaysRun = false;
            MenuBarDisabled = false;
            CrimeQuery = true;
        }

        public bool IsMaximized
        {
            get { return m_IsFullScreen; }
            set { SetProperty(ref m_IsFullScreen, value); }
        }

        public MouseProperty Mouse
        {
            get { return m_Mouse; }
            set { SetProperty(ref m_Mouse, value); }
        }

        public ResolutionProperty FullScreenResolution
        {
            get { return m_FullScreenResolution; }
            set
            {
                if (!Resolutions.IsValidFullScreenResolution(value))
                    return;
                SetProperty(ref m_FullScreenResolution, value);
            }
        }

        public ResolutionProperty WindowResolution
        {
            get { return m_WindowResolution; }
            set { SetProperty(ref m_WindowResolution, value); }
        }

        public ResolutionProperty PlayWindowGumpResolution
        {
            get { return m_WorldGumpResolution; }
            set
            {
                if (!Resolutions.IsValidPlayWindowResolution(value))
                    SetProperty(ref m_WorldGumpResolution, new ResolutionProperty());
                SetProperty(ref m_WorldGumpResolution, value);
            }
        }

        public bool PlayWindowPixelDoubling
        {
            get { return m_PlayWindowPixelDoubling; }
            set { SetProperty(ref m_PlayWindowPixelDoubling, value); }
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

        public int SpeechColor
        {
            get { return m_SpeechColor; }
            set { SetProperty(ref m_SpeechColor, Clamp(value, 0, HueData.HueCount - 1)); }
        }

        public int EmoteColor
        {
            get { return m_EmoteColor; }
            set { SetProperty(ref m_EmoteColor, Clamp(value, 0, HueData.HueCount - 1)); }
        }

        public int PartyPrivateMsgColor
        {
            get { return m_PartyMsgPrivateColor; }
            set { SetProperty(ref m_PartyMsgPrivateColor, Clamp(value, 0, HueData.HueCount - 1)); }
        }

        public int PartyMsgColor
        {
            get { return m_PartyMsgColor; }
            set { SetProperty(ref m_PartyMsgColor, Clamp(value, 0, HueData.HueCount - 1)); }
        }

        public int GuildMsgColor
        {
            get { return m_GuildMsgColor; }
            set { SetProperty(ref m_GuildMsgColor, Clamp(value, 0, HueData.HueCount - 1)); }
        }

        public bool IgnoreGuildMsg
        {
            get { return m_IgnoreGuildMsg; }
            set { SetProperty(ref m_IgnoreGuildMsg, value); }
        }

        public int AllianceMsgColor
        {
            get { return m_AllianceMsgColor; }
            set { SetProperty(ref m_AllianceMsgColor, Clamp(value, 0, HueData.HueCount - 1)); }
        }

        public bool IgnoreAllianceMsg
        {
            get { return m_IgnoreAllianceMsg; }
            set { SetProperty(ref m_IgnoreAllianceMsg, value); }
        }

        public bool CrimeQuery
        {
            get { return m_CrimeQuery; }
            set { SetProperty(ref m_CrimeQuery, value); }
        }
    }
}
