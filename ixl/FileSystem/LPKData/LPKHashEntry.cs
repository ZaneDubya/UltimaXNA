namespace InterXLib.FileSystem.LPKData
{
    public class LPKHashEntry
    {
        public uint SecondHash;
        public uint BlockIndex;
        public uint NextHashInSequence;

        public LPKHashEntry(BinaryFileReader reader)
        {
            SecondHash = reader.ReadUInt();
            BlockIndex = reader.ReadUInt();
            NextHashInSequence = reader.ReadUInt();
        }

        public LPKHashEntry(uint second_hash, uint block_index, uint next_hash)
        {
            SecondHash = second_hash;
            BlockIndex = block_index;
            NextHashInSequence = next_hash;
        }

        public void Serialize(BinaryFileWriter writer)
        {
            writer.Write((uint)SecondHash);
            writer.Write((uint)BlockIndex);
            writer.Write((uint)NextHashInSequence);
        }

        public const int SizeInBytes = 3 * 4;

        public static void SerializeAsEmpty(BinaryFileWriter writer)
        {
            writer.Write((uint)0);
            writer.Write((uint)0);
            writer.Write((uint)LPK.NoHash);
        }
    }
}
