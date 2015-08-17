using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UltimaXNA.Ultima.IO
{
    class MulFileIndex : FileIndexBase
    {
        private string idxPath;
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
            idxPath = FileManager.GetFilePath(idxFile);
            Length = length;
            patchFile = patch_file;  
        }

        protected override FileIndexEntry[] ReadEntries()
        {

            var entries = new List<FileIndexEntry>();

            var length = (int)((new FileInfo(idxPath).Length / 3) / 4);

            using (var index = new FileStream(idxPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var bin = new BinaryReader(index);

                var count = (int)(index.Length / 12);

                for (var i = 0; i < count && i < length; ++i)
                {
                    var entry = new FileIndexEntry
                    {
                        lookup = bin.ReadInt32(),
                        length = bin.ReadInt32(),
                        extra = bin.ReadInt32()
                    };

                    entries.Add(entry);
                }

                for (var i = count; i < length; ++i)
                {
                    var entry = new FileIndexEntry
                    {
                        lookup = -1,
                        length = -1,
                        extra = -1
                    };

                    entries.Add(entry);
                }
            }

            Entry5D[] patches = VerData.Patches;

            for (int i = 0; i < patches.Length; ++i)
            {
                Entry5D patch = patches[i];

                if (patch.file == patchFile && patch.index >= 0 && patch.index < entries.Count)
                {
                    var entry = entries.ElementAt(patch.index);
                    entry.lookup = patch.lookup;
                    entry.length = patch.length | (1 << 31);
                    entry.extra = patch.extra;
                }
            }

            return entries.ToArray();
        }

    }
}
