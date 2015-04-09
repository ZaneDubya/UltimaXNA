#region Usings

using System;

#endregion

namespace UltimaXNA.Configuration
{
    public static class Settings
    {
        private static readonly SettingsFile m_file;

        static Settings()
        {
            m_file = new SettingsFile("settings.json");

            Debug = new DebugSettings(m_file);
            Server = new ServerSettings(m_file);
            UltimaOnline = new UltimaOnlineSettings(m_file);
            Game = new GameSettings(m_file);
        }

        public static bool IsSettingsFileCreated
        {
            get { return m_file.Exists; }
        }

        public static DebugSettings Debug
        {
            get;
            private set;
        }

        public static ServerSettings Server
        {
            get;
            private set;
        }

        public static UltimaOnlineSettings UltimaOnline
        {
            get;
            private set;
        }

        public static GameSettings Game
        {
            get;
            private set;
        }

        internal static void Save()
        {
            m_file.Save();
        }

        public static T OpenSection<T>()
            where T : SettingsBase
        {
            T settings = (T)Activator.CreateInstance(typeof(T), m_file);
            return settings;
        }
    }
}