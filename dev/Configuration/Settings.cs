using System;
namespace UltimaXNA.Data
{
    public static class Settings
    {
        private static readonly SettingsFile _file;

        static Settings()
        {
            _file = new SettingsFile("settings.json");

            Debug = new DebugSettings(_file);
            Server = new ServerSettings(_file);
            UltimaOnline = new UltimaOnlineSettings(_file);
            Game = new GameSettings(_file);
        }

        public static bool IsSettingsFileCreated
        {
            get { return _file.Exists; }
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
            _file.Save();
        }

        public static T OpenSection<T>()
            where T : SettingsBase
        {
            var settings = (T)Activator.CreateInstance(typeof(T), _file);
            return settings;
        }
    }
}