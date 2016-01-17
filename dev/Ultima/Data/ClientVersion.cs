using System;
using System.Diagnostics;
using System.IO;
using UltimaXNA.Ultima.IO;

namespace UltimaXNA.Ultima.Data
{
    public static class ClientVersion
    {
        private static readonly Version m_UnknownClientVersion = new Version("0.0.0.0");
        private static readonly Version m_ExtendedFeaturesVersion = new Version("6.0.14.2");
        private static readonly Version m_ConvertedToUOPVersion = new Version("7.0.24.0");

        private static Version m_Version = null;
        private static bool m_VersionUnlocked = false; // set to true after server sends 0xbd packet.

        public static Version Version
        {
            get
            {
                return new Version(6, 0, 6, 2);
                //my client.exe don't have version's information
                if (m_Version == null)
                {
                    string clientExe = FileManager.GetPath("client.exe");

                    if (File.Exists(clientExe))
                    {
                        FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(clientExe);
                        m_Version = new Version(
                            fileVersionInfo.FileMajorPart,
                            fileVersionInfo.FileMinorPart,
                            fileVersionInfo.FileBuildPart,
                            fileVersionInfo.FilePrivatePart);
                    }
                    else
                    {
                        m_Version = m_UnknownClientVersion;
                    }
                }

                if (m_VersionUnlocked)
                    return m_Version;
                else
                    return m_UnknownClientVersion;
            }
        }

        /// <summary>
        /// Call after server sends version request packet - 0xbd
        /// </summary>
        public static void UnlockVersion()
        {
            m_VersionUnlocked = true;
        }

        public static void ClearVersion()
        {
            m_Version = null;
            m_VersionUnlocked = false;
        }

        public static bool IsUnknownClientVersion
        {
            get { return Version == m_UnknownClientVersion; }
        }

        public static bool HasExtendedClientFeatures
        {
            get { return Version >= m_ExtendedFeaturesVersion; }
        }

        public static bool IsUopFormat
        {
            get { return Version >= m_ConvertedToUOPVersion; }
        }
    }
}
