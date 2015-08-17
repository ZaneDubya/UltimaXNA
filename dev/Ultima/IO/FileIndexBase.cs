using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UltimaXNA.Core.IO;

namespace UltimaXNA.Ultima.IO
{
    public abstract class FileIndexBase
    {
        protected FileIndexEntry[] m_Index;
        protected Stream m_Stream;

        public FileIndexEntry[] Index { get { return m_Index; } }
        public Stream Stream { get { return m_Stream; } }

        public string DataPath { get; private set; }
        public int Length { get; set; }

        protected abstract FileIndexEntry[] ReadEntries();

        protected FileIndexBase(string dataPath)
        {
            DataPath = dataPath;
        }

        protected FileIndexBase(string dataPath, int length)
        {
            Length = length;
            DataPath = dataPath;
        }

        public void Open()
        {
            m_Index = ReadEntries();
            Length = m_Index.Length;
        }

        public BinaryFileReader Seek(int index, out int length, out int extra, out bool patched)
        {
            if (Stream == null)
            {
                m_Stream = new FileStream(DataPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }

            if (index < 0 || index >= m_Index.Length)
            {
                length = extra = 0;
                patched = false;
                return null;
            }

            FileIndexEntry e = m_Index[index];

            if (e.lookup < 0)
            {
                length = extra = 0;
                patched = false;
                return null;
            }

            length = e.length & 0x7FFFFFFF;
            extra = e.extra;

            if ((e.length & 0xFF000000) != 0)
            {
                patched = true;

                VerData.Stream.Seek(e.lookup, SeekOrigin.Begin);
                return new BinaryFileReader(new BinaryReader(VerData.Stream));
            }
            else if (m_Stream == null)
            {
                length = extra = 0;
                patched = false;
                return null;
            }

            patched = false;

            m_Stream.Position = e.lookup;
            return new BinaryFileReader(new BinaryReader(m_Stream));
        }
    }
}
