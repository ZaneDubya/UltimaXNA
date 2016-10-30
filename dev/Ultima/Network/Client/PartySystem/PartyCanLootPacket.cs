using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client.PartySystem {
    public class PartyCanLootPacket : SendPacket {
        public PartyCanLootPacket(bool isLootable) : base(0xbf, "Party Can Loot") {
            Stream.Write((short)6);
            Stream.Write((byte)6);
            Stream.Write(isLootable);
        }
    }
}