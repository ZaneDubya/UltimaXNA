using System.IO;

namespace UltimaXNA.Ultima.IO
{
    public class VerData
    {
        private static FileIndexEntry5D[] m_Patches;
        private static Stream m_Stream;

        public static Stream Stream { get { return m_Stream; } }
        public static FileIndexEntry5D[] Patches { get { return m_Patches; } }

        static VerData()
        {
            string path = FileManager.GetFilePath("verdata.mul");

            if (!File.Exists(path))
            {
                m_Patches = new FileIndexEntry5D[0];
                m_Stream = Stream.Null;
            }
            else
            {
                m_Stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryReader bin = new BinaryReader(m_Stream);

                m_Patches = new FileIndexEntry5D[bin.ReadInt32()];

                for (int i = 0; i < m_Patches.Length; ++i)
                {
                    m_Patches[i].file = bin.ReadInt32();
                    m_Patches[i].index = bin.ReadInt32();
                    m_Patches[i].lookup = bin.ReadInt32();
                    m_Patches[i].length = bin.ReadInt32();
                    m_Patches[i].extra = bin.ReadInt32();
                }
            }
        }
    }
}