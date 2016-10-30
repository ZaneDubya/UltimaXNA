using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client.PartySystem {
    public class PartyQueryLocs : SendPacket {
        public PartyQueryLocs() : base(240, "Query Party Locations") {
            Stream.Write((byte)0);
        }
    }
}