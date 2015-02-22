namespace InterXLib.FileSystem.LPKData
{
    public class LPKBlockEntry
    {
        public uint PtrFile;
        public uint FileSize;
        public uint FileSizeCompressed;
        public uint Flags;

        public LPKBlockEntry(BinaryFileReader reader)
        {
            PtrFile = reader.ReadUInt();
            FileSize = reader.ReadUInt();
            FileSizeCompressed = reader.ReadUInt();
            Flags = reader.ReadUInt();
        }

        public LPKBlockEntry(uint ptr_file, uint file_size, uint file_size_compressed, uint flags)
        {
            PtrFile = ptr_file;
            FileSize = file_size;
            FileSizeCompressed = file_size_compressed;
            Flags = flags;
        }

        public void Serialize(BinaryFileWriter writer)
        {
            writer.Write((uint)PtrFile);
            writer.Write((uint)FileSize);
            writer.Write((uint)FileSizeCompressed);
            writer.Write((uint)Flags);
        }

        public const int SizeInBytes = 4 * 4;
    }
}
