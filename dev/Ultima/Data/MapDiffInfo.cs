using UltimaXNA.Core.Network;

namespace UltimaXNA.Ultima.Data
{
    public class MapDiffInfo
    {
        public readonly int MapCount;
        public readonly int[] MapPatches;
        public readonly int[] StaticPatches;

        public MapDiffInfo(PacketReader reader)
        {
            MapCount = reader.ReadInt32();
            MapPatches = new int[MapCount];
            StaticPatches = new int[MapCount];
            for (int i = 0; i < MapCount; i++)
            {
                StaticPatches[i] = reader.ReadInt32();
                MapPatches[i] = reader.ReadInt32();
            }
        }
    }
}
