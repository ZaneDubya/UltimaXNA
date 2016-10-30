using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client.PartySystem {
    public class PartyQueryStatsPacket : SendPacket {
        public PartyQueryStatsPacket(int Serial) : base(0x34, "Query Stats", 10) {
            Stream.Write(0xFFFFFFFF);
            Stream.Write(0xDEDEDEDE);
            Stream.Write((byte)4);
            Stream.Write(Serial);
        }
    }
}