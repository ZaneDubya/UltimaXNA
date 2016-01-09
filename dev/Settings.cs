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
using System.Collections.Generic;
using System.Linq;
using UltimaXNA.Configuration;
using UltimaXNA.Core.Configuration;

#endregion usings

namespace UltimaXNA
{
    public class Settings
    {
        private static readonly Settings s_Instance;
        private static readonly SettingsFile s_File;

        private DebugSettings m_Debug;
        private EngineSettings m_Engine;
        private GumpSettings m_Gumps;
        private UserInterfaceSettings m_UI;
        private LoginSettings m_Login;
        private UltimaOnlineSettings m_UltimaOnline;
        private AudioSettings m_Audio;
        private MacroSettings m_Macro;

        static Settings()
        {
            s_Instance = new Settings();
            s_File = new SettingsFile("settings.cfg");

            s_Instance.m_Debug = CreateOrOpenSection<DebugSettings>(DebugSettings.SectionName);
            s_Instance.m_Login = CreateOrOpenSection<LoginSettings>(LoginSettings.SectionName);
            s_Instance.m_UltimaOnline = CreateOrOpenSection<UltimaOnlineSettings>(UltimaOnlineSettings.SectionName);
            s_Instance.m_Engine = CreateOrOpenSection<EngineSettings>(EngineSettings.SectionName);
            s_Instance.m_UI = CreateOrOpenSection<UserInterfaceSettings>(UserInterfaceSettings.SectionName);
            s_Instance.m_Gumps = CreateOrOpenSection<GumpSettings>(GumpSettings.SectionName);
            s_Instance.m_Audio = CreateOrOpenSection<AudioSettings>(AudioSettings.SectionName);
            s_Instance.m_Macro = CreateOrOpenSection<MacroSettings>(MacroSettings.SectionName);
            s_File.Load();
        }

        public static bool IsSettingsFileCreated
        {
            get { return s_File.Exists; }
        }

        public static DebugSettings Debug
        {
            get { return s_Instance.m_Debug; }
        }

        public static LoginSettings Login
        {
            get { return s_Instance.m_Login; }
        }

        public static UltimaOnlineSettings UltimaOnline
        {
            get { return s_Instance.m_UltimaOnline; }
        }

        public static EngineSettings Engine
        {
            get { return s_Instance.m_Engine; }
        }

        public static GumpSettings Gumps
        {
            get { return s_Instance.m_Gumps; }
        }

        public static UserInterfaceSettings UserInterface
        {
            get { return s_Instance.m_UI; }
        }

        public static AudioSettings Audio
        {
            get { return s_Instance.m_Audio; }
        }

        public static MacroSettings Macro
        {
            get { return s_Instance.m_Macro; }
        }

        internal static void Save()
        {
            s_File.Save();
        }

        public static T CreateOrOpenSection<T>(string sectionName)
            where T : ASettingsSection, new()
        {
            T section = s_File.CreateOrOpenSection<T>(sectionName);

            // Resubscribe incase this is called for a section 2 times.
            section.Invalidated -= OnSectionInvalidated;
            section.Invalidated += OnSectionInvalidated;
            section.PropertyChanged -= OnSectionPropertyChanged;
            section.PropertyChanged += OnSectionPropertyChanged;

            return section;
        }

        private static void OnSectionPropertyChanged(object sender, EventArgs e)
        {
            s_File.InvalidateDirty();
        }

        private static void OnSectionInvalidated(object sender, EventArgs e)
        {
            s_File.InvalidateDirty();
        }

        public static string[] fromList<T>(List<T> list)
        {
            string[] arrayList = new string[list.Count];
            for (int i = 0; i < arrayList.Length; i++)
            {
                arrayList[i] = list[i].ToString();
            }
            return arrayList;
        }

        public static List<string> fromList<T>(List<T> list, bool stringList = true)
        {
            return fromList(list).ToList();
        }
    }
}