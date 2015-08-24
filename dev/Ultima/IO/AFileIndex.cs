/***************************************************************************
 *   FileIndexEntry5D.cs
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.IO;
using UltimaXNA.Core.IO;
#endregion

namespace UltimaXNA.Ultima.IO
{
    public abstract class AFileIndex
    {
        protected FileIndexEntry3D[] m_Index;
        protected Stream m_Stream;

        public FileIndexEntry3D[] Index { get { return m_Index; } }
        public Stream Stream { get { return m_Stream; } }

        public string DataPath { get; private set; }
        public int Length { get; set; }

        protected abstract FileIndexEntry3D[] ReadEntries();

        protected AFileIndex(string dataPath)
        {
            DataPath = dataPath;
        }

        protected AFileIndex(string dataPath, int length)
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

            FileIndexEntry3D e = m_Index[index];

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
