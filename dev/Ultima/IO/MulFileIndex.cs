/***************************************************************************
 *   MulFileIndex.cs
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
 *   And on code from OpenUO: https://github.com/jeffboulanger/OpenUO
 *      Copyright (c) 2011 OpenUO Software Team.
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#endregion

namespace UltimaXNA.Ultima.IO
{
    class MulFileIndex : AFileIndex
    {
        private readonly string IndexPath;
        public int patchFile { get; set; }

        /// <summary>
        /// Creates a reference to an index file. (Ex: anim.idx)
        /// </summary>
        /// <param name="idxFile">Name of .idx file in UO base directory.</param>
        /// <param name="mulFile">Name of .mul file that this index file provides an index for.</param>
        /// <param name="length">Number of indexes in this index file.</param>
        /// <param name="patch_file">Index to patch data in Versioning.</param>
        public MulFileIndex(string idxFile, string mulFile, int length, int patch_file)
            : base(mulFile)
        {
            IndexPath = FileManager.GetFilePath(idxFile);
            Length = length;
            patchFile = patch_file;  
            Open();
        }

        protected override FileIndexEntry3D[] ReadEntries()
        {
            if (!File.Exists(IndexPath) || !File.Exists(DataPath))
            {
                return new FileIndexEntry3D[0];
            }

            List<FileIndexEntry3D> entries = new List<FileIndexEntry3D>();

            int length = (int)((new FileInfo(IndexPath).Length / 3) / 4);

            using (FileStream index = new FileStream(IndexPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryReader bin = new BinaryReader(index);

                int count = (int)(index.Length / 12);

                for (int i = 0; i < count && i < length; ++i)
                {
                    FileIndexEntry3D entry = new FileIndexEntry3D
                    {
                        lookup = bin.ReadInt32(),
                        length = bin.ReadInt32(),
                        extra = bin.ReadInt32()
                    };

                    entries.Add(entry);
                }

                for (int i = count; i < length; ++i)
                {
                    FileIndexEntry3D entry = new FileIndexEntry3D
                    {
                        lookup = -1,
                        length = -1,
                        extra = -1
                    };

                    entries.Add(entry);
                }
            }

            FileIndexEntry5D[] patches = VerData.Patches;

            for (int i = 0; i < patches.Length; ++i)
            {
                FileIndexEntry5D patch = patches[i];

                if (patch.file == patchFile && patch.index >= 0 && patch.index < entries.Count)
                {
                    FileIndexEntry3D entry = entries.ElementAt(patch.index);
                    entry.lookup = patch.lookup;
                    entry.length = patch.length | (1 << 31);
                    entry.extra = patch.extra;
                }
            }

            return entries.ToArray();
        }

    }
}
