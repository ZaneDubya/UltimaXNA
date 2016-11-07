using UltimaXNA.Core.Network;
using UltimaXNA.Ultima.Data;

namespace UltimaXNA.Ultima.Network.Server.GeneralInfo {
    /// <summary>
    /// Subcommand 0x1B: the contents of a spellbook.
    /// </summary>
    class SpellBookContentsInfo : IGeneralInfo {
        public readonly SpellbookData Spellbook;

        public SpellBookContentsInfo(PacketReader reader) {
            ushort unknown = reader.ReadUInt16(); // always 1
            Serial serial = reader.ReadInt32();
            ushort itemID = reader.ReadUInt16();
            ushort spellbookType = reader.ReadUInt16(); // 1==regular, 101=necro, 201=paladin, 401=bushido, 501=ninjitsu, 601=spellweaving
            ulong spellBitfields = reader.ReadUInt32() + (((ulong)reader.ReadUInt32()) << 32); // first bit of first byte = spell #1, second bit of first byte = spell #2, first bit of second byte = spell #8, etc 
            Spellbook = new SpellbookData(serial, itemID, spellbookType, spellBitfields);
        }
    }
}
