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
#region Usings
using System;
using UltimaXNA.Core.Configuration;
using UltimaXNA.Configuration;
#endregion

namespace UltimaXNA
{
    public class Settings
    {
        private static readonly Settings m_Instance;
        private static readonly SettingsFile m_File;

        private DebugSettings m_Debug;
        private GameSettings m_Game;
        private GumpSettings m_Gumps;
        private WorldSettings m_World;
        private ServerSettings m_Server;
        private UltimaOnlineSettings m_UltimaOnline;

        static Settings()
        {
            m_Instance = new Settings();
            m_File = new SettingsFile("settings.cfg");

            m_Instance.m_Debug = CreateOrOpenSection<DebugSettings>(DebugSettings.SectionName);
            m_Instance.m_Server = CreateOrOpenSection<ServerSettings>(ServerSettings.SectionName);
            m_Instance.m_UltimaOnline = CreateOrOpenSection<UltimaOnlineSettings>(UltimaOnlineSettings.SectionName);
            m_Instance.m_Game = CreateOrOpenSection<GameSettings>(GameSettings.SectionName);
            m_Instance.m_World = CreateOrOpenSection<WorldSettings>(WorldSettings.SectionName);
            m_Instance.m_Gumps = CreateOrOpenSection<GumpSettings>(GumpSettings.SectionName);
            
            m_File.Load();
        }

        public static bool IsSettingsFileCreated
        {
            get { return m_File.Exists; }
        }

        public static DebugSettings Debug
        {
            get { return m_Instance.m_Debug; }
        }

        public static ServerSettings Server
        {
            get { return m_Instance.m_Server; }
        }

        public static UltimaOnlineSettings UltimaOnline
        {
            get { return m_Instance.m_UltimaOnline; }
        }

        public static GameSettings Game
        {
            get { return m_Instance.m_Game; }
        }

        public static GumpSettings Gumps
        {
            get { return m_Instance.m_Gumps; }
        }

        public static WorldSettings World
        {
            get { return m_Instance.m_World; }
        }
        
        internal static void Save()
        {
            m_File.Save();
        }

        public static T CreateOrOpenSection<T>(string sectionName)
            where T : ASettingsSection, new()
        {
            T section = m_File.CreateOrOpenSection<T>(sectionName);

            // Resubscribe incase this is called for a section 2 times.
            section.Invalidated -= OnSectionInvalidated;
            section.Invalidated += OnSectionInvalidated;
            section.PropertyChanged -= OnSectionPropertyChanged;
            section.PropertyChanged += OnSectionPropertyChanged;

            return section;
        }

        private static void OnSectionPropertyChanged(object sender, EventArgs e)
        {
            m_File.InvalidateDirty();
        }

        private static void OnSectionInvalidated(object sender, EventArgs e)
        {
            m_File.InvalidateDirty();
        }
    }
}