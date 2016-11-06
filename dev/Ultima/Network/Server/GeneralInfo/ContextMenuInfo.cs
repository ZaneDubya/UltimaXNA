using UltimaXNA.Core.Network;
using UltimaXNA.Ultima.Data;

namespace UltimaXNA.Ultima.Network.Server.GeneralInfo {
    /// <summary>
    /// Subcommand 0x14: A context menu.
    /// </summary>
    class ContextMenuInfo : IGeneralInfo {
        public readonly ContextMenuData Menu;

        public ContextMenuInfo(PacketReader reader) {
            reader.ReadByte(); // unknown, always 0x00
            int subcommand = reader.ReadByte(); // 0x01 for 2D, 0x02 for KR
            Menu = new ContextMenuData(reader.ReadInt32());
            int contextMenuChoiceCount = reader.ReadByte();
            for (int i = 0; i < contextMenuChoiceCount; i++) {
                int iUniqueID = reader.ReadUInt16();
                int iClilocID = reader.ReadUInt16() + 3000000;
                int iFlags = reader.ReadUInt16(); // 0x00=enabled, 0x01=disabled, 0x02=arrow, 0x20 = color
                int iColor = 0;
                if ((iFlags & 0x20) == 0x20) {
                    iColor = reader.ReadUInt16();
                }
                Menu.AddItem(iUniqueID, iClilocID, iFlags, iColor);
            }
        }
    }
}
