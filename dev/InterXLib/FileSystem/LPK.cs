using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterXLib.FileSystem.LPKData;
using System.IO;

namespace InterXLib.FileSystem
{
    public class LPK
    {
        private BinaryFileReader m_Reader;

        protected LPKHeader m_Header;
        protected LPKHashEntry[] m_HashEntries;
        protected LPKBlockEntry[] m_BlockEntries;

        private const uint c_RetrieveAllData = 0xFFFFFFFF;
        public const uint NoHash = 0xFFFFFFFF;
        public uint HashCount = 0x1000; // 4k entries

        public int CollisionsCount
        {
            get
            {
                int count = 0;

                for (int i = 0; i < m_Header.Hashcount; i++)
                {
                    LPKHashEntry hash = m_HashEntries[i];
                    if (hash.NextHashInSequence != NoHash)
                        count++;
                }

                return count;
            }
        }

        public uint FileCount
        {
            get
            {
                return m_Header.BlockCount;
            }
        }

        public byte[] GetFileBytes(string path)
        {
            LPKHashEntry hash = InternalGetHashEntryFromPath(path);
            if (hash == null)
                return null;
            BinaryFileReader reader = InternalGetFileBinaryReader(hash);
            byte[] data = reader.ReadBytes((int)m_BlockEntries[hash.BlockIndex].FileSize);
            return data;
        }

        public BinaryFileReader GetFileBinaryReader(string path)
        {
            LPKHashEntry hash = InternalGetHashEntryFromPath(path);
            if (hash == null)
                return null;
            BinaryFileReader reader = InternalGetFileBinaryReader(hash);
            return reader;
        }

        private BinaryFileReader InternalGetFileBinaryReader(LPKHashEntry hash, uint offset = 0, uint length = c_RetrieveAllData)
        {
            LPKBlockEntry block = m_BlockEntries[hash.BlockIndex];
            if (block.FileSizeCompressed == 0)
            {
                if (length == c_RetrieveAllData)
                    length = block.FileSize;

                if (offset + length > block.FileSize || offset < 0)
                    Logging.Fatal("Attempt to read outside of requested data block!");

                // add file pointer to offset.
                offset = offset + block.PtrFile + m_Header.PtrFiles;

                m_Reader.Position = offset;
                return m_Reader;
            }
            else
            {
                m_Reader.Position = m_Header.PtrFiles + block.PtrFile;
                byte[] compressed_data = m_Reader.ReadBytes((int)block.FileSizeCompressed);
                byte[] data = Compression.LZF.Decompress(compressed_data, (int)block.FileSize);
                MemoryStream mem_stream = new MemoryStream(data);
                return new BinaryFileReader(mem_stream);
            }
        }

        private LPKHashEntry InternalGetHashEntryFromPath(string path)
        {
            string filename = path.ToLower();
            uint hash1 = CRC32.ComputeChecksum(filename) % HashCount;
            uint hash2 = CRC32.ComputeChecksumReverse(filename);

            LPKHashEntry hash = m_HashEntries[hash1];
            while (hash.SecondHash != hash2)
            {
                if (hash.NextHashInSequence == NoHash)
                    return null;
                hash = m_HashEntries[hash.NextHashInSequence];
                // Sanity check ???
                // if we call for a path that does not exist this will run endlessly or return an empty file...
                // so we should check that we haven't reached an empty hash...
            }

            return hash;
        }

        private string[] InternalGetFileStrings(string path)
        {
            LPKHashEntry hash = InternalGetHashEntryFromPath(path);
            BinaryFileReader reader = InternalGetFileBinaryReader(hash);
            byte[] data = reader.ReadBytes((int)m_BlockEntries[hash.BlockIndex].FileSize);
            string[] strings = Encoding.ASCII.GetString(data).Split('\n');
            return strings;
        }

        public LPK(string path)
        {
            if (path == null || path == string.Empty)
                Logging.Fatal("Path to packed file system cannot be empty.");
            if (!System.IO.File.Exists(path))
                Logging.Fatal("Path to packed file system must exist");

            m_Reader = InterXLib.Serialize.OpenReader(path);
            m_Header = new LPKHeader(m_Reader);

            m_HashEntries = new LPKHashEntry[m_Header.Hashcount];
            if (m_Header.HashTableSizeCompressed != 0)
            {
                byte[] hash_table_compressed = m_Reader.ReadBytes((int)m_Header.HashTableSizeCompressed);
                byte[] hash_table = Compression.LZF.Decompress(hash_table_compressed, (int)m_Header.Hashcount * LPKHashEntry.SizeInBytes);
                using (MemoryStream mem_stream = new MemoryStream(hash_table))
                {
                    BinaryFileReader reader = new BinaryFileReader(mem_stream);
                    for (int i = 0; i < m_Header.Hashcount; i++)
                        m_HashEntries[i] = new LPKHashEntry(reader);
                }
            }
            else
            {
                for (int i = 0; i < m_Header.Hashcount; i++)
                    m_HashEntries[i] = new LPKHashEntry(m_Reader);
            }

            m_BlockEntries = new LPKBlockEntry[m_Header.BlockCount];
            if (m_Header.HashTableSizeCompressed != 0)
            {
                byte[] block_table_compressed = m_Reader.ReadBytes((int)m_Header.BlockTableSizeCompressed);
                byte[] block_table = Compression.LZF.Decompress(block_table_compressed, (int)m_Header.BlockCount * LPKBlockEntry.SizeInBytes);
                using (MemoryStream mem_stream = new MemoryStream(block_table))
                {
                    BinaryFileReader reader = new BinaryFileReader(mem_stream);
                    for (int i = 0; i < m_Header.BlockCount; i++)
                        m_BlockEntries[i] = new LPKBlockEntry(reader);
                }
            }
            else
            {
                for (int i = 0; i < m_Header.BlockCount; i++)
                    m_BlockEntries[i] = new LPKBlockEntry(m_Reader);
            }
        }

        protected LPK()
        {

        }

        public void Explode(string explode_directory = "Exploded")
        {
            string filelist_filename = "filelist.txt";
            string[] files = InternalGetFileStrings(filelist_filename);

            for (int i = 0; i < files.Length; i++)
            {
                string file_to_expolode = files[i].ToLower();
                LPKHashEntry hash = InternalGetHashEntryFromPath(file_to_expolode);
                if (hash == null)
                {
                    Logging.Fatal("Unknown file on FileSystem Explode().");
                }
                else
                {
                    LPKBlockEntry block = m_BlockEntries[hash.BlockIndex];
                    BinaryFileReader reader = InternalGetFileBinaryReader(hash);
                    byte[] data = reader.ReadBytes((int)block.FileSize);
                    string dir_path = explode_directory + '\\' + System.IO.Path.GetDirectoryName(files[i]);
                    string filename = System.IO.Path.GetFileName(files[i]);
                    if (dir_path.Length > 0)
                    {
                        System.IO.Directory.CreateDirectory(dir_path);
                    }
                    BinaryFileWriter writer = InterXLib.Serialize.OpenWriter(dir_path + '\\' + filename);
                    writer.Write(data);
                    writer.Close();
                    writer = null;
                }
            }
        }

        public void CloseFileSystem()
        {
            m_HashEntries = null;
            m_BlockEntries = null;
            m_Reader = null;
        }
    }
}
