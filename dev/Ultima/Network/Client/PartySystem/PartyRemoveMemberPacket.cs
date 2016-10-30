using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client.PartySystem {
    public class PartyRemoveMemberPacket : SendPacket {
        public PartyRemoveMemberPacket(Serial _serial) : base(0xbf, "Remove Party Member") {
            Stream.Write((short)6);
            Stream.Write((byte)2);
            Stream.Write(_serial);
        }
    }
}