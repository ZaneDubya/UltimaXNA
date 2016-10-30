using UltimaXNA.Core.Network.Packets;
using UltimaXNA.Ultima.World;

namespace UltimaXNA.Ultima.Network.Client.PartySystem {
    public class PartyQuitPacket : SendPacket {
        public PartyQuitPacket() : base(0xbf, "Quit Party") {
            Stream.Write((short)6);
            Stream.Write((byte)2);
            Stream.Write(WorldModel.Entities.GetPlayerEntity().Serial);
        }
    }
}