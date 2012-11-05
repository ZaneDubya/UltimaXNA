/***************************************************************************
 *   FileIndexClint.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code by Clint.XNA: http://www.runuo.com/community/threads/91272/
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.IO;
using System.Runtime.InteropServices;
#endregion

namespace UltimaXNA.UltimaData
{
    [StructLayout(LayoutKind.Sequential)]
    struct IndexEntry
    {
        public int Lookup;
        public int Length;
        public int Extra;
    }

    class FileIndexClint
    {
        public BinaryReader BinaryReader;
        public IndexEntry[] IndexEntries;

        public void Seek(int index)
        {
            this.BinaryReader.BaseStream.Seek(IndexEntries[index].Lookup, SeekOrigin.Begin);
        }

        public void Seek(int index, out int extra)
        {
            Seek(index);

            extra = this.IndexEntries[index].Extra;
        }

        public unsafe FileIndexClint(string indexFile, string mulFile)
        {
            if (!FileManager.Exists(indexFile))
            {
                throw new FileNotFoundException(indexFile);
            }

            if (!FileManager.Exists(mulFile))
            {
                throw new FileNotFoundException(mulFile);
            }

            using (FileStream stream = FileManager.GetFile(indexFile))
            {
                this.BinaryReader = new BinaryReader(FileManager.GetFile(mulFile));

                this.IndexEntries = new IndexEntry[(int)(stream.Length / 12)];

                fixed (IndexEntry* pIndexEntries = this.IndexEntries)
                {
                    NativeMethods.Read(stream.SafeFileHandle, pIndexEntries, (int)stream.Length);
                }
            }
        }

        public ushort[] ReadUInt16Array(int count)
        {
            byte[] tempData = BinaryReader.ReadBytes(count * 2);
            ushort[] data = new ushort[count];
            System.Buffer.BlockCopy(tempData, 0, data, 0, count * 2);
            return data;
        }

        public static ushort[] ReadUInt16Array(BinaryReader bin, int count)
        {
            byte[] tempData = bin.ReadBytes(count * 2);
            ushort[] data = new ushort[count];
            System.Buffer.BlockCopy(tempData, 0, data, 0, count * 2);
            return data;
        }

        public static int[] ReadInt32Array(BinaryReader bin, int count)
        {
            byte[] tempData = bin.ReadBytes(count * 4);
            int[] data = new int[count];
            System.Buffer.BlockCopy(tempData, 0, data, 0, count * 4);
            return data;
        }
    }
}