/***************************************************************************
 *   Verdata.cs
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
#endregion

namespace UltimaXNA.UltimaData
{
    public class Versioning
    {
        private static Entry5D[] m_Patches;
        private static Stream m_Stream;

        public static Stream Stream { get { return m_Stream; } }
        public static Entry5D[] Patches { get { return m_Patches; } }

        static Versioning()
        {
            string path = FileManager.GetFilePath("verdata.mul");

            if (!File.Exists(path))
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