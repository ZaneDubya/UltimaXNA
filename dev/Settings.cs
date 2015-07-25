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
        private static readonly Settings s_Instance;
        private static readonly SettingsFile s_File;

        private DebugSettings m_Debug;
        private GameSettings m_Game;
        private GumpSettings m_Gumps;
        private WorldSettings m_World;
        private ServerSettings m_Server;
        private UltimaOnlineSettings m_UltimaOnline;
        private AudioSettings m_Audio;

        static Settings()
        {
            s_Instance = new Settings();
            s_File = new SettingsFile("settings.cfg");

            s_Instance.m_Debug = CreateOrOpenSection<DebugSettings>(DebugSettings.SectionName);
            s_Instance.m_Server = CreateOrOpenSection<ServerSettings>(ServerSettings.SectionName);
            s_Instance.m_UltimaOnline = CreateOrOpenSection<UltimaOnlineSettings>(UltimaOnlineSettings.SectionName);
            s_Instance.m_Game = CreateOrOpenSection<GameSettings>(GameSettings.SectionName);
            s_Instance.m_World = CreateOrOpenSection<WorldSettings>(WorldSettings.SectionName);
            s_Instance.m_Gumps = CreateOrOpenSection<GumpSettings>(GumpSettings.SectionName);
            s_Instance.m_Audio = CreateOrOpenSection<AudioSettings>(AudioSettings.SectionName);
            
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

        public static ServerSettings Server
        {
            get { return s_Instance.m_Server; }
        }

        public static UltimaOnlineSettings UltimaOnline
        {
            get { return s_Instance.m_UltimaOnline; }
        }

        public static GameSettings Game
        {
            get { return s_Instance.m_Game; }
        }

        public static GumpSettings Gumps
        {
            get { return s_Instance.m_Gumps; }
        }

        public static WorldSettings World
        {
            get { return s_Instance.m_World; }
        }

        public static AudioSettings Audio
        {
            get { return s_Instance.m_Audio; }
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
    }
}