using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterXLib;

namespace InterXLib.FileSystem.Processors
{
    public abstract class AProcessor
    {
        public abstract bool TryProcess(string filename, byte[] data, bool allow_compression_of_files, out ProcessedFile processed_file);
        public abstract bool ExcludeThisFileFromLPK(string filepath);

        protected bool InternalCheckExtension(string filename, string expected)
        {
            string ext = System.IO.Path.GetExtension(filename).ToLower();
            return (ext == expected);
        }

        protected void AddValueToByteList(List<byte> data, ushort value)
        {
            for (int i = 0; i < 2; i++)
            {
                data.Add((byte)(value & 0xFF));
                value = (ushort)(value >> 8);
            }
        }

        protected void AddValueToByteList(List<byte> data, uint value)
        {
            for (int i = 0; i < 4; i++)
            {
                data.Add((byte)(value & 0xFF));
                value = (uint)(value >> 8);
            }
        }
    }

    public class ProcessedFile
    {
        public string Filename = null;
        public byte[] ProcessedData = null;
        public byte[] CompressedData = null;

        public bool IsCompressed { get { return CompressedData != null; } }

        public ProcessedFile(string filename, byte[] data, bool attempt_compression)
        {
            Filename = filename;
            ProcessedData = data;
            if (attempt_compression)
                TryCompress();
        }

        public void TryCompress()
        {
            if (Compression.LZF.ReccomendCompression(ProcessedData))
            {
                Compression.LZF.Compress(ProcessedData, out CompressedData);
            }
        }
    }
}
