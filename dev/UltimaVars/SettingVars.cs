using Microsoft.Xna.Framework;
using UltimaXNA.Entity;
using UltimaXNA.UltimaPackets;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaWorld;
using UltimaXNA.IniHandler;

namespace UltimaXNA.UltimaVars
{
    public class SettingVars
    {
        static IniFile iniFile = new IniFile("Settings.ini");

        static bool m_alwaysRun = iniFile.GetBoolean("Control", "AlwaysRun", false);
        static string m_ServerIP = iniFile.GetString("Server", "ServerIP", "localhost");
        static int m_ServerPort = iniFile.GetInt32("Server", "ServerPort", 2593);
        static string m_LastAccount = iniFile.GetString("Server", "LastAccount", "");
        static string m_UOData = iniFile.GetString("Files", "UOData", null);

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
                iniFile.WriteValue("Server", "LastAccount", m_LastAccount);
            }
        }
        public static string UOData
        {
            get { return m_UOData; }
            set
            {
                m_UOData = value;
                iniFile.WriteValue("Server", "LastAccount", m_UOData);
            }
        }
    }
}