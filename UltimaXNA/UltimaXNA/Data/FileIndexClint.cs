﻿/***************************************************************************
 *   FileIndexClint.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
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

namespace UltimaXNA.Data
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
            using (FileStream stream = FileManager.GetFile(indexFile))
            {
                this.BinaryReader = new BinaryReader(FileManager.GetFile(mulFile));

                this.IndexEntries = new IndexEntry[(int)(stream.Length / 12)];

                fixed (IndexEntry* pIndexEntries = this.IndexEntries)
                {
                    NativeMethods.Read(stream.SafeFileHandle.DangerousGetHandle(), pIndexEntries, (int)stream.Length);
                }
            }
        }
    }
}