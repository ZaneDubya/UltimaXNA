using UltimaXNA.Core.Network.Packets;

namespace UltimaXNA.Ultima.Network.Client.PartySystem {
    public class PartyPublicMessagePacket : SendPacket {
        public PartyPublicMessagePacket(string text) : base(0xbf, "Public Party Message") {
            Stream.Write((short)6);
            Stream.Write((byte)4);
            Stream.WriteBigUniNull(text);
            Stream.Write((short)0);
        }
    }
}