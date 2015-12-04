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
        public const string SectionName = "ui";

        private ResolutionProperty m_FullScreenResolution;
        private ResolutionProperty m_WindowResolution;
        private ResolutionProperty m_WorldGumpResolution;
        private bool m_PlayWindowPixelDoubling;
        private bool m_IsFullScreen;
        private MouseProperty m_Mouse;
        private bool m_AlwaysRun;
        private bool m_MenuBarDisabled;

        private int m_SpeechColor;
        private int m_EmoteColor;
        private int m_PartyMsgColor;
        private int m_GuildMsgColor;
        private bool m_IgnoreGuildMsg;
        private int m_AllianceMsgColor;
        private bool m_IgnoreAllianceMsg;

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
    }
}
