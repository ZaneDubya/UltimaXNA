using UltimaXNA.Core.Network.Packets;
using UltimaXNA.Ultima.World.Entities.Mobiles;

namespace UltimaXNA.Ultima.Network.Client.PartySystem {
    public class PartyDeclinePacket : SendPacket {
        public PartyDeclinePacket(Mobile Leader) : base(0xbf, "Party Join Decline") {
            Stream.Write((short)6);
            Stream.Write((byte)9);
            Stream.Write(Leader.Serial);
        }
    }
}