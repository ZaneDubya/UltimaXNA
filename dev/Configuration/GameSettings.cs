/***************************************************************************
 *   GameSettings.cs
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
    public sealed class GameSettings : ASettingsSection
    {
        public const string SectionName = "game";

        private bool m_AutoSelectLastCharacter;
        private string m_LastCharacterName;
        private bool m_IsVSyncEnabled;
        private bool m_IsFixedTimeStep;
        private int m_SpeechColor;
        private int m_EmoteColor;
        private int m_PartyMsgColor;
        private int m_GuildMsgColor;
        private bool m_IgnoreGuildMsg;
        private int m_AllianceMsgColor;
        private bool m_IgnoreAllianceMsg;

        public GameSettings()
        {
            IsFixedTimeStep = true;
            IsVSyncEnabled = false;
            LastCharacterName = string.Empty;
            AutoSelectLastCharacter = false;
        }

        public bool IsFixedTimeStep
        {
            get { return m_IsFixedTimeStep; }
            set { SetProperty(ref m_IsFixedTimeStep, value); }
        }

        public bool IsVSyncEnabled
        {
            get { return m_IsVSyncEnabled; }
            set { SetProperty(ref m_IsVSyncEnabled, value); }
        }

        public string LastCharacterName
        {
            get { return m_LastCharacterName; }
            set { SetProperty(ref m_LastCharacterName, value); }
        }

        public bool AutoSelectLastCharacter
        {
            get { return m_AutoSelectLastCharacter; }
            set { SetProperty(ref m_AutoSelectLastCharacter, value); }
        }

        public int SpeechColor
        {
            get { return m_SpeechColor; }
            set { SetProperty(ref m_SpeechColor, value); }
        }

        public int EmoteColor
        {
            get { return m_EmoteColor; }
            set { SetProperty(ref m_EmoteColor, value); }
        }

        public int PartyMsgColor
        {
            get { return m_PartyMsgColor; }
            set { SetProperty(ref m_PartyMsgColor, value); }
        }

        public int GuildMsgColor
        {
            get { return m_GuildMsgColor; }
            set { SetProperty(ref m_GuildMsgColor, value); }
        }

        public bool IgnoreGuildMsg
        {
            get { return m_IgnoreGuildMsg; }
            set { SetProperty(ref m_IgnoreGuildMsg, value); }
        }

        public int AllianceMsgColor
        {
            get { return m_AllianceMsgColor; }
            set { SetProperty(ref m_AllianceMsgColor, value); }
        }

        public bool IgnoreAllianceMsg
        {
            get { return m_IgnoreAllianceMsg; }
            set { SetProperty(ref m_IgnoreAllianceMsg, value); }
        }
    }
}