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
        bool m_IsConsoleEnabled;
        bool m_ShowFps;
        bool m_LogPackets;

        public DebugSettings()
        {
            LogPackets = false;
            IsConsoleEnabled = true;
            ShowFps = true;
        }

        /// <summary>
        /// If true, all received packets will be logged to Tracer.Debug, and any active Tracer listeners (console, debug.txt file logger)
        /// </summary>
        public bool LogPackets
        {
            get { return m_LogPackets; }
            set { SetProperty(ref m_LogPackets, value); }
        }

        /// <summary>
        /// If true, FPS should display either in the window caption or in the game window. (not currently enabled).
        /// </summary>
        public bool ShowFps
        {
            get { return m_ShowFps; }
            set { SetProperty(ref m_ShowFps, value); }
        }

        /// <summary>
        /// If true, a console window which will display debug and error messages should appear at runtime. This may not work in Release configurations.
        /// </summary>
        public bool IsConsoleEnabled
        {
            get { return m_IsConsoleEnabled; }
            set { SetProperty(ref m_IsConsoleEnabled, value); }
        }
    }
}