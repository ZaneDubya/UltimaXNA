using UltimaXNA.Core.Network;
using UltimaXNA.Ultima.Data;

namespace UltimaXNA.Ultima.Network.Server.GeneralInfo {
    /// <summary>
    /// Subcommand 0x1D: The revision hash of a custom house.
    /// </summary>
    class HouseRevisionInfo : IGeneralInfo {
        public readonly HouseRevisionState HouseRevisionState;

        public HouseRevisionInfo(PacketReader reader) {
            Serial s = reader.ReadInt32();
            int hash = reader.ReadInt32();
            HouseRevisionState = new HouseRevisionState(s, hash);
        }
    }
}
