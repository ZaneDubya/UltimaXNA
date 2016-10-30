using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client {
    public class PartyRemoveMember : SendPacket {
        public PartyRemoveMember(Serial _serial) : base(0xbf, "Remove Party Member") {
            Stream.Write((short)6);
            Stream.Write((byte)2);
            Stream.Write(_serial);
        }
    }
}