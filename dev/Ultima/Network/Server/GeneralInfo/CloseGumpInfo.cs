using UltimaXNA.Core.Network;

namespace UltimaXNA.Ultima.Network.Server.GeneralInfo {
    /// <summary>
    /// Subcommand 0x04: Close a generic gump.
    /// </summary>
    class CloseGumpInfo : IGeneralInfo {
        public readonly int GumpTypeID;
        public readonly int GumpButtonID;

        public CloseGumpInfo(PacketReader reader) {
            GumpTypeID = reader.ReadInt32();
            GumpButtonID = reader.ReadInt32();
        }
    }
}
