namespace InterXLib.FileSystem.LPKData
{
    public class LPKHeader
    {
        public uint Hashcount;
        public uint BlockCount;
        public uint PtrHashTable;
        public uint PtrBlocks;
        public uint PtrFiles;

        public uint HashTableSizeCompressed = 0;
        public uint BlockTableSizeCompressed = 0;

        public LPKHeader(BinaryFileReader reader)
        {
            uint version_expected = InterXLib.Library.FourCharsToUInt("LPK2");
            uint version = reader.ReadUInt();

            if (version != version_expected)
                Logging.Fatal("File is not a lpk file.");

            Hashcount = reader.ReadUInt();
            BlockCount = reader.ReadUInt();
            PtrHashTable = reader.ReadUInt();
            PtrBlocks = reader.ReadUInt();
            PtrFiles = reader.ReadUInt();

            HashTableSizeCompressed = reader.ReadUInt();
            BlockTableSizeCompressed = reader.ReadUInt();
        }

        public LPKHeader(uint hash_count, uint block_count)
        {
            Hashcount = hash_count;
            BlockCount = block_count;
            PtrHashTable = LPKHeader.SizeInBytes;
            PtrBlocks = PtrHashTable + LPKHashEntry.SizeInBytes * Hashcount;
            PtrFiles = PtrBlocks + LPKBlockEntry.SizeInBytes * BlockCount;
        }

        public void SetCompressedDataSizes(uint hash_table_compressed, uint block_table_compressed)
        {
            HashTableSizeCompressed = hash_table_compressed;
            BlockTableSizeCompressed = block_table_compressed;
            PtrBlocks = PtrHashTable + hash_table_compressed;
            PtrFiles = PtrBlocks + block_table_compressed;
        }

        public void Serialize(BinaryFileWriter writer)
        {
            writer.Write((uint)InterXLib.Library.FourCharsToUInt("LPK2"));

            writer.Write((uint)Hashcount);
            writer.Write((uint)BlockCount);
            writer.Write((uint)PtrHashTable);
            writer.Write((uint)PtrBlocks);
            writer.Write((uint)PtrFiles);

            writer.Write((uint)HashTableSizeCompressed);
            writer.Write((uint)BlockTableSizeCompressed);
        }

        public const int SizeInBytes = 4 * 8;
    }
}
