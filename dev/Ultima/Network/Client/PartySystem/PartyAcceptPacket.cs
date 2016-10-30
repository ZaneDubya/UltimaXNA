using UltimaXNA.Core.Network.Packets;
using UltimaXNA.Ultima.World.Entities.Mobiles;

namespace UltimaXNA.Ultima.Network.Client.PartySystem {
    public class PartyAcceptPacket : SendPacket {
        public PartyAcceptPacket(Mobile Leader) : base(0xbf, "Party Join Accept") {
            Stream.Write((short)6);
            Stream.Write((byte)8);
            Stream.Write(Leader.Serial);
        }
    }
}