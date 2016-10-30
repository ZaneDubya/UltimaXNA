using UltimaXNA.Core.Network;
using UltimaXNA.Ultima.Network.Client.PartySystem;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.World.Entities.Mobiles;

namespace UltimaXNA.Ultima.Player.Partying {
    public class PartyMember {
        public bool IsLeader { get; set; }
        public bool IsLootable { get; set; }   // only working on client Entity
        public Mobile Player { get; set; }

        public PartyMember(Serial serial, bool isLeader) {
            IsLeader = isLeader;
            Player = WorldModel.Entities.GetObject<Mobile>(serial, false);
            INetworkClient m_Network = ServiceRegistry.GetService<INetworkClient>();
            m_Network.Send(new PartyQueryStatsPacket(serial)); // I THINK CHECK FOR STATUS ??
        }
    }
}
