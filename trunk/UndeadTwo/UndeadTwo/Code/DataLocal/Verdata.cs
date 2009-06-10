#region File Description & Usings
//-----------------------------------------------------------------------------
// SharedMethods.cs
//
// Based on UltimaSDK
//-----------------------------------------------------------------------------
using System.IO;
#endregion

namespace UltimaXNA.DataLocal
{
    public class Verdata
    {
        private static Entry5D[] m_Patches;
        private static Stream m_Stream;

        public static Stream Stream { get { return m_Stream; } }
        public static Entry5D[] Patches { get { return m_Patches; } }

        static Verdata()
        {
            string path = FileManager.GetFilePath("verdata.mul");

            if (path == null)
            {
                m_Patches = new Entry5D[0];
                m_Stream = Stream.Null;
            }
            else
            {
                m_Stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryReader bin = new BinaryReader(m_Stream);

                m_Patches = new Entry5D[bin.ReadInt32()];

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

    public struct Entry5D
    {
        public int file;
        public int index;
        public int lookup;
        public int length;
        public int extra;
    }
}