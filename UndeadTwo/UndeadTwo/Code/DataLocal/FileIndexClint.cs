#region File Description & Usings
//-----------------------------------------------------------------------------
// FileIndexClint.cs
//
// Based on UltimaSDK, modifications by ClintXNA
//-----------------------------------------------------------------------------
using System.IO;
using System.Runtime.InteropServices;
#endregion

namespace UndeadClient.DataLocal
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