#region Usings

using UltimaXNA.UltimaData;

#endregion

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

        protected override string Comments
        {
            get { return @"This section is responsible for settings related to the Ultima Online client."; }
        }

        public string DataDirectory
        {
            get { return GetValue(FileManager.DataPath); }
            set { SetValue(value, "Location of the Ultima Online installation UltimaXNA is using."); }
        }
    }
}