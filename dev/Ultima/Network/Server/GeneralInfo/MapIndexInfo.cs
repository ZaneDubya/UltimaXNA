using UltimaXNA.Core.Network;

namespace UltimaXNA.Ultima.Network.Server.GeneralInfo {
    /// <summary>
    /// Subcommand 0x08: The index of the map the player is located within.
    /// </summary>
    class MapIndexInfo : IGeneralInfo {
        public readonly byte MapID;
        public MapIndexInfo(PacketReader reader) {
            MapID = reader.ReadByte();
        }
    }
}
