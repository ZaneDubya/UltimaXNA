#region Usings
using UltimaXNA.Core.Configuration;
#endregion

namespace UltimaXNA.Configuration
{
    public sealed class UltimaOnlineSettings : ASettingsSection
    {
        public const string SectionName = "ultimaOnline";

        private string m_DataDirectory;
        private byte[] m_ClientVersion;

        public UltimaOnlineSettings()
        {
            ClientVersion = new byte[] {6, 0, 6, 2};
        }

        public byte[] ClientVersion
        {
            get { return m_ClientVersion; }
            set { SetProperty(ref m_ClientVersion, value); }
        }
        
        public string DataDirectory
        {
            get { return m_DataDirectory; }
            set { SetProperty(ref m_DataDirectory, value); }
        }
    }
}