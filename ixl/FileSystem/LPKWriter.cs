using InterXLib.FileSystem.LPKData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InterXLib.FileSystem
{
    public class LPKWriter : LPK
    {
        private class OrphanedHash
        {
            public uint FirstHash;
            public LPKHashEntry HashEntry;

            public OrphanedHash(uint first_hash, LPKHashEntry hash_entry)
            {
                FirstHash = first_hash;
                HashEntry = hash_entry;
            }
        }

        LPKWriter(uint file_count)
            : base()
        {
            m_Header = new LPKHeader(HashCount, file_count);
            m_HashEntries = new LPKHashEntry[m_Header.Hashcount];
            for (int i = 0; i < m_Header.Hashcount; i++)
                m_HashEntries[i] = null;

            m_BlockEntries = new LPKBlockEntry[m_Header.BlockCount];
            for (int i = 0; i < m_Header.BlockCount; i++)
                m_BlockEntries[i] = null;
        }

        public static void CreatePackedFileSystem(string in_directory_prefix, string in_directory, string out_file_path, bool compress_headers = true, bool allow_compression_of_files = true, uint hash_count = 0x1000)
        {
            string filelist_filename = "filelist.txt";
            string worklpk_filename = "work.lpk";

            // Create the local content processor
            Processors.AllProcessors processor = new Processors.AllProcessors();

            // get all files to include in this packed file system
            string basepath = AppDomain.CurrentDomain.BaseDirectory + in_directory_prefix;
            IEnumerable<string> all_files = InterXLib.Library.GetFilesInPath(basepath + in_directory + @"\").ToArray<string>();
            List<string> lstFilenames = new List<string>();
            lstFilenames.Add(filelist_filename);
            foreach (string file in all_files)
            {
                if (processor.ExcludeThisFileFromLPK(file))
                {
                    // exclude it... write out a list of files to exclude?
                }
                else
                {
                    lstFilenames.Add(file);
                }
            }

            // create the file system. It has num_files (first file is 'filelist.txt' which contains all files in the packed file system.
            LPKWriter fs = new LPKWriter((uint)(lstFilenames.Count));
            // open the work writer
            BinaryFileWriter work_file_contents = InterXLib.Serialize.OpenWriter(worklpk_filename);
            // list of any files that don't fit in the current system (their hash is already taken).
            List<OrphanedHash> orphaned_hashes = new List<OrphanedHash>();

            // create and save the 'filelist.txt' file. It should be the first file inserted into the hash array.
            if (lstFilenames.Count > 0)
            {
                StringBuilder sbFilelist = new StringBuilder();
                for (int i = 0; i < lstFilenames.Count; i++)
                {
                    if (sbFilelist.Length > 0)
                        sbFilelist.Append("\n");
                    string istr = lstFilenames[i];
                    if (istr.StartsWith(basepath))
                        istr = istr.Remove(0, basepath.Length);
                    sbFilelist.Append(istr);
                }
                File.WriteAllText(filelist_filename, sbFilelist.ToString());
            }

            // create all hashes and blocks and write the contents of the files out to a work file.
            for (int i = 0; i < lstFilenames.Count; i++)
            {
                string filename = lstFilenames[i];
                if (filename.StartsWith(basepath))
                    filename = filename.Remove(0, basepath.Length);
                filename = filename.ToLower();
                uint crc = CRC32.ComputeChecksum(filename);
                uint crc2 = CRC32.ComputeChecksumReverse(filename);
                LPKHashEntry entry = new LPKHashEntry(crc2, (uint)i, LPK.NoHash);

                if (fs.m_HashEntries[crc % hash_count] == null)
                    fs.m_HashEntries[crc % hash_count] = entry;
                else
                    orphaned_hashes.Add(new OrphanedHash(crc, entry));

                // write the data from the files to the work file.
                // if the data can be 'processed', write the processed data instead.
                uint ptr_to_file = (uint)work_file_contents.Position;
                byte[] file_data = System.IO.File.ReadAllBytes(lstFilenames[i]);
                Processors.ProcessedFile processed_file = null;

                if (processor.TryProcess(lstFilenames[i], file_data, allow_compression_of_files, out processed_file))
                {
                    if (processed_file.IsCompressed)
                    {
                        file_data = processed_file.CompressedData;
                        fs.m_BlockEntries[i] = new LPKBlockEntry(ptr_to_file, (uint)processed_file.ProcessedData.Length, (uint)processed_file.CompressedData.Length, 0);
                    }
                    else
                    {
                        file_data = processed_file.ProcessedData;
                        fs.m_BlockEntries[i] = new LPKBlockEntry(ptr_to_file, (uint)file_data.Length, 0, 0);
                    }
                    work_file_contents.Write(file_data);
                }
                else
                {
                    fs.m_BlockEntries[i] = new LPKBlockEntry(ptr_to_file, (uint)file_data.Length, 0, 0);
                    work_file_contents.Write(file_data);
                }
            }

            // place the orphaned hashes in the hash entries array.
            for (int i = 0; i < orphaned_hashes.Count; i++)
            {
                uint crc = orphaned_hashes[i].FirstHash;
                LPKHashEntry entry = orphaned_hashes[i].HashEntry;

                LPKHashEntry last_entry = fs.m_HashEntries[crc % hash_count];
                while (last_entry.NextHashInSequence != NoHash)
                {
                    last_entry = fs.m_HashEntries[last_entry.NextHashInSequence % hash_count];
                    // !!! Sanity check to see that we don't loop forever?
                }

                int next_hash_to_check = 0;
                // find the first open spot for the orphaned entry.
                for (int j = next_hash_to_check; j < hash_count; j++)
                {
                    if (fs.m_HashEntries[j] == null)
                    {
                        fs.m_HashEntries[j] = entry;
                        last_entry.NextHashInSequence = (uint)j;
                        next_hash_to_check = j + 1;
                        break;
                    }
                }
            }

            // close the work writer
            work_file_contents.Close();
            work_file_contents = null;

            // delete the filelist.
            System.IO.File.Delete(filelist_filename);

            // get the work file contents and then delete the work file.
            byte[] all_file_data = System.IO.File.ReadAllBytes(worklpk_filename);
            System.IO.File.Delete(worklpk_filename);

            while (System.IO.File.Exists(out_file_path))
            {
                try
                {
                    System.IO.File.Delete(out_file_path);
                }
                catch
                {
                    System.Threading.Thread.Sleep(1);
                }
            }
            BinaryFileWriter out_file_writer = InterXLib.Serialize.OpenWriter(out_file_path);
            fs.InternalSerializeHeaders(out_file_writer, compress_headers);
            out_file_writer.Write(all_file_data);

            out_file_writer.Close();
            out_file_writer = null;
        }

        private void InternalSerializeHeaders(BinaryFileWriter writer, bool compress)
        {
            byte[] hash_entry_data = InternalGetHashEntryBytes();
            byte[] block_entry_data = InternalGetBlockEntryBytes();

            if (compress)
            {
                byte[] hash_entries_compressed, block_entries_compressed;
                Compression.LZF.Compress(hash_entry_data, out hash_entries_compressed);
                Compression.LZF.Compress(block_entry_data, out block_entries_compressed);
                m_Header.SetCompressedDataSizes((uint)hash_entries_compressed.Length, (uint)block_entries_compressed.Length);

                m_Header.Serialize(writer);
                writer.Write(hash_entries_compressed);
                writer.Write(block_entries_compressed);
            }
            else
            {
                m_Header.HashTableSizeCompressed = 0;
                m_Header.BlockTableSizeCompressed = 0;

                m_Header.Serialize(writer);
                writer.Write(hash_entry_data);
                writer.Write(block_entry_data);
            }
        }

        private byte[] InternalGetHashEntryBytes()
        {
            byte[] hash_entry_data;
            BinaryFileWriter mem_writer = new BinaryFileWriter(new MemoryStream(), false);
            for (int i = 0; i < m_Header.Hashcount; i++)
            {
                if (m_HashEntries[i] != null)
                    m_HashEntries[i].Serialize(mem_writer);
                else
                    LPKHashEntry.SerializeAsEmpty(mem_writer);
            }
            hash_entry_data = ((MemoryStream)mem_writer.UnderlyingStream).ToArray();
            mem_writer.Close();
            mem_writer = null;
            return hash_entry_data;
        }

        private byte[] InternalGetBlockEntryBytes()
        {
            byte[] data;
            BinaryFileWriter mem_writer = new BinaryFileWriter(new MemoryStream(), false);
            for (int i = 0; i < m_Header.BlockCount; i++)
                m_BlockEntries[i].Serialize(mem_writer);
            data = ((MemoryStream)mem_writer.UnderlyingStream).ToArray();
            mem_writer.Close();
            mem_writer = null;
            return data;
        }
    }
}
