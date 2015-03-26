using Microsoft.Xna.Framework;
using UltimaXNA.UltimaEntities;
using UltimaXNA.UltimaPackets;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaWorld;
using UltimaXNA.Core;

namespace UltimaXNA.UltimaVars
{
    public class SettingVars
    {
        private static string m_Path = "Settings.ini";
        private static IniFile m_IniFile;

        private static bool m_alwaysRun; // = m_IniFile.GetBoolean("Control", "AlwaysRun", false);
        private static string m_ServerIP; // = m_IniFile.GetString("Server", "ServerIP", "localhost");
        private static int m_ServerPort; // = m_IniFile.GetInt32("Server", "ServerPort", 2593);
        private static string m_LastAccount; // = m_IniFile.GetString("Server", "LastAccount", "");
        private static string m_UOData; // = m_IniFile.GetString("Files", "UOData", null);

        public static void Load()
        {
            // load the ini file.
            m_IniFile = new IniFile(m_Path);

            // load the various settings.
            // Is there a way we could do this programatically? Maybe a IniSetting class?
            m_alwaysRun = m_IniFile.GetBoolean("Control", "AlwaysRun", false);
            m_ServerIP = m_IniFile.GetString("Server", "ServerIP", "localhost");
            m_ServerPort = m_IniFile.GetInt32("Server", "ServerPort", 2593);
            m_LastAccount = m_IniFile.GetString("Server", "LastAccount", string.Empty);
            m_UOData = m_IniFile.GetString("Files", "UOData", string.Empty);
        }

        public static void Save()
        {
            // only save if we've previously loaded the ini file.
            if (m_IniFile != null)
            {
                m_IniFile.WriteValue("Control", "AlwaysRun", m_alwaysRun);
                m_IniFile.WriteValue("Server", "ServerIP", m_ServerIP);
                m_IniFile.WriteValue("Server", "ServerPort", m_ServerPort);
                m_IniFile.WriteValue("Server", "LastAccount", m_LastAccount);
                m_IniFile.WriteValue("Files", "UOData", m_UOData);
            }
        }

        public static bool AlwaysRun
        {
            get { return m_alwaysRun; }
            set { m_alwaysRun = value; }
        }

        public static string ServerIP
        {
            get { return m_ServerIP; }
            set { m_ServerIP = value; }
        }

        public static int ServerPort
        {
            get { return m_ServerPort; }
            set { m_ServerPort = value; }
        }

        public static string LastAccount
        {
            get { return m_LastAccount; }
            set
            {
                m_LastAccount = value;
            }
        }
        public static string UOData
        {
            get { return m_UOData; }
            set
            {
                m_UOData = value;
            }
        }
    }
}