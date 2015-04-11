#region Usings

using System;

#endregion

namespace UltimaXNA.Configuration
{
    public class Settings
    {
        private static readonly Settings m_Instance;
        private static readonly SettingsFile m_File;

        private DebugSettings m_Debug;
        private GameSettings m_Game;
        private ServerSettings m_Server;
        private UltimaOnlineSettings m_UltimaOnline;

        static Settings()
        {
            m_File = new SettingsFile("settings.json");
            m_Instance = new Settings();

            m_Instance.m_Debug = CreateOrOpenSection<DebugSettings>(DebugSettings.SectionName);
            m_Instance.m_Server = CreateOrOpenSection<ServerSettings>(ServerSettings.SectionName);
            m_Instance.m_UltimaOnline = CreateOrOpenSection<UltimaOnlineSettings>(UltimaOnlineSettings.SectionName);
            m_Instance.m_Game = CreateOrOpenSection<GameSettings>(GameSettings.SectionName);
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
        
        internal static void Save()
        {
            m_File.Save();
        }

        public static T CreateOrOpenSection<T>(string sectionName)
            where T : SettingsSectionBase, new()
        {
            var section =  m_File.CreateOrOpenSection<T>(sectionName);

            // Resubscribe incase this is called for a section 2 times.
            section.Invalidated -= OnSectionInvalidated;
            section.Invalidated += OnSectionInvalidated;
            section.PropertyChanged -= OnSectionPropertyChanged;
            section.PropertyChanged += OnSectionPropertyChanged;

            return section;
        }

        private static void OnSectionPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            m_File.InvalidateDirty();
        }

        private static void OnSectionInvalidated(object sender, EventArgs e)
        {
            m_File.InvalidateDirty();
        }
    }
}