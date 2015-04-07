using UltimaXNA.UltimaData;

namespace UltimaXNA.Data
{
    public sealed class UltimaOnlineSettings : SettingsBase
    {
        private const string UltimaOnline = "ultimaOnline";

        internal UltimaOnlineSettings(SettingsFile file)
            : base(file)
        {
        }

        protected override string Name
        {
            get { return UltimaOnline; }
        }

        public string DataDirectory
        {
            get { return GetValue(FileManager.DataPath); }
            set { SetValue(value); }
        }
    }
}