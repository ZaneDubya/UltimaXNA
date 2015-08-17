/***************************************************************************
 *   DebugSettings.cs
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
    public sealed class DebugSettings : ASettingsSection
    {
        public const string SectionName = "debug";

        private bool m_IsConsoleEnabled;
        private bool m_ShowFps;
        private bool m_ShowDataRead;
        private bool m_ShowDataReadBreakdown;
        private bool m_ShowUIOutlines;
        private bool m_LogPackets;

        public DebugSettings()
        {
            IsConsoleEnabled = true;
            ShowFps = true;
        }

        public bool LogPackets
        {
            get { return m_LogPackets; }
            set { SetProperty(ref m_LogPackets, value); }
        }
        
        public bool ShowUIOutlines
        {
            get { return m_ShowUIOutlines; }
            set { SetProperty(ref m_ShowUIOutlines, value); }
        }

        public bool ShowDataReadBreakdown
        {
            get { return m_ShowDataReadBreakdown; }
            set { SetProperty(ref m_ShowDataReadBreakdown, value); }
        }

        public bool ShowDataRead
        {
            get { return m_ShowDataRead; }
            set { SetProperty(ref m_ShowDataRead, value); }
        }

        public bool ShowFps
        {
            get { return m_ShowFps; }
            set { SetProperty(ref m_ShowFps, value); }
        }

        public bool IsConsoleEnabled
        {
            get { return m_IsConsoleEnabled; }
            set { SetProperty(ref m_IsConsoleEnabled, value); }
        }
    }
}