using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client.PartySystem {
    internal class PartyAddMemberPacket : SendPacket {
        public PartyAddMemberPacket() : base(0xbf, "Add Party Member") {
            Stream.Write((short)6);
            Stream.Write((byte)1);
            Stream.Write(0);
        }
    }
}