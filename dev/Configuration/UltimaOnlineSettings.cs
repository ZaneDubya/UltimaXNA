#region Usings



#endregion

namespace UltimaXNA.Configuration
{
    public sealed class UltimaOnlineSettings : SettingsSectionBase
    {
        public const string SectionName = "ultimaOnline";

        private string m_DataDirectory;
        
        public string DataDirectory
        {
            get { return m_DataDirectory; }
            set { SetProperty(ref m_DataDirectory, value); }
        }
    }
}