/***************************************************************************
 *   Settings.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

#region usings
using System;
using UltimaXNA.Configuration;
using UltimaXNA.Core.Configuration;
#endregion

namespace UltimaXNA
{
    public class Settings
    {
        // === Instance ===============================================================================================
        readonly DebugSettings m_Debug;
        readonly EngineSettings m_Engine;
        readonly GumpSettings m_Gumps;
        readonly UserInterfaceSettings m_UI;
        readonly LoginSettings m_Login;
        readonly UltimaOnlineSettings m_UltimaOnline;
        readonly AudioSettings m_Audio;

        Settings()
        {
            m_Debug = CreateOrOpenSection<DebugSettings>();
            m_Login = CreateOrOpenSection<LoginSettings>();
            m_UltimaOnline = CreateOrOpenSection<UltimaOnlineSettings>();
            m_Engine = CreateOrOpenSection<EngineSettings>();
            m_UI = CreateOrOpenSection<UserInterfaceSettings>();
            m_Gumps = CreateOrOpenSection<GumpSettings>();
            m_Audio = CreateOrOpenSection<AudioSettings>();
        }

        // === Static Settings properties =============================================================================
        public static DebugSettings Debug => s_Instance.m_Debug;
        public static LoginSettings Login => s_Instance.m_Login;
        public static UltimaOnlineSettings UltimaOnline => s_Instance.m_UltimaOnline;
        public static EngineSettings Engine => s_Instance.m_Engine;
        public static GumpSettings Gumps => s_Instance.m_Gumps;
        public static UserInterfaceSettings UserInterface => s_Instance.m_UI;
        public static AudioSettings Audio => s_Instance.m_Audio;

        static readonly Settings s_Instance;
        static readonly SettingsFile s_File;

        static Settings()
        {
            s_File = new SettingsFile("settings.cfg");
            s_Instance = new Settings();
            s_File.Load();
        }

        public static void Save()
        {
            s_File.Save();
        }

        public static T CreateOrOpenSection<T>()
            where T : ASettingsSection, new()
        {
            string sectionName = typeof(T).Name;
            T section = s_File.CreateOrOpenSection<T>(sectionName);
            // Resubscribe incase this is called for a section 2 times.
            section.Invalidated -= OnSectionInvalidated;
            section.Invalidated += OnSectionInvalidated;
            section.PropertyChanged -= OnSectionPropertyChanged;
            section.PropertyChanged += OnSectionPropertyChanged;
            return section;
        }

        static void OnSectionPropertyChanged(object sender, EventArgs e)
        {
            s_File.InvalidateDirty();
        }

        static void OnSectionInvalidated(object sender, EventArgs e)
        {
            s_File.InvalidateDirty();
        }
    }
}