using UltimaXNA.Core.Network;

namespace UltimaXNA.Ultima.Network.Server.GeneralInfo {
    /// <summary>
    /// Subcommand 0x18: The count of map diffs that were received.
    /// </summary>
    class MapDiffInfo : IGeneralInfo {
        public readonly int MapDiffsCount;

        public MapDiffInfo(PacketReader reader) {
            MapDiffsCount = reader.ReadInt32();
            for (int i = 0; i < MapDiffsCount; i++) {
                int mapPatches = reader.ReadInt32();
                int staticPatches = reader.ReadInt32();
            }
        }
    }
}
