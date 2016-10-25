using System.Diagnostics;
using System.IO;
using UltimaXNA.Ultima.IO;

namespace UltimaXNA.Ultima.Data
{
    public static class ClientVersion
    {
        private static readonly byte[] m_UnknownClientVersion = { 0, 0, 0, 0 };
        private static readonly byte[] m_ExtendedAddItemToContainer = { 6, 0, 1, 7 };
        private static readonly byte[] m_ExtendedFeaturesVersion = { 6, 0, 14, 2 };
        private static readonly byte[] m_ConvertedToUOPVersion = { 7, 0, 24, 0 };
        private static byte[] m_ClientExeVersion;

        public static byte[] ClientExe {
            get {
                if (m_ClientExeVersion == null) {
                    string path = FileManager.GetPath("client.exe");
                    if (File.Exists(path)) {
                        FileVersionInfo clientExeVersion = FileVersionInfo.GetVersionInfo(path);
                        m_ClientExeVersion = new byte[] {
                            (byte)clientExeVersion.FileMajorPart, (byte)clientExeVersion.FileMinorPart,
                            (byte)clientExeVersion.FileBuildPart, (byte)clientExeVersion.FilePrivatePart };
                    }
                    else {
                        m_ClientExeVersion = m_UnknownClientVersion;
                    }
                }

                return m_ClientExeVersion;
            }
        }

        public static bool InstallationIsUopFormat => GreaterThanOrEqualTo(ClientExe, m_ConvertedToUOPVersion);

        public static bool HasExtendedFeatures(byte[] version) => GreaterThanOrEqualTo(version, m_ExtendedFeaturesVersion);

        public static bool HasExtendedAddItemPacket(byte[] version) => GreaterThanOrEqualTo(version, m_ExtendedAddItemToContainer);

        /// <summary> Compare two arrays of equal size. Returns true if first parameter array is greater than or equal to second. </summary>
        private static bool GreaterThanOrEqualTo(byte[] a, byte[] b) {
            if (a.Length != b.Length)
                return false;
            int index = 0;
            while (index < a.Length) {
                if (a[index] > b[index])
                    return true;
                if (a[index] < b[index])
                    return false;
                index++;
            }
            return true;
        }
    }
}
