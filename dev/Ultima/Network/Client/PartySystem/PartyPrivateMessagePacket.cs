using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client.PartySystem {
    public class PartyPrivateMessagePacket : SendPacket {
        public PartyPrivateMessagePacket(Serial memberSerial, string PMsg) : base(0xbf, "Private Party Message") {
            Stream.Write((short)6);
            Stream.Write((byte)3);
            Stream.Write(memberSerial);
            Stream.WriteBigUniNull(PMsg);
            Stream.Write((short)0);
        }
    }
}