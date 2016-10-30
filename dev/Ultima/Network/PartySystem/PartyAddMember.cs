using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client {
    internal class PartyAddMember : SendPacket {
        public PartyAddMember() : base(0xbf, "Add Party Member") {
            Stream.Write((short)6);
            Stream.Write((byte)1);
            Stream.Write(0);
        }
    }
}