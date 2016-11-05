using UltimaXNA.Core.Network;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.World.Entities.Mobiles;

namespace UltimaXNA.Ultima.Player.Partying
{
    public class PartyMember
    {
        public bool IsLeader { get; set; }
        public Mobile Mobile { get; set; }

        public PartyMember(Serial serial, bool isLeader)
        {
            IsLeader = isLeader;
            Mobile = WorldModel.Entities.GetObject<Mobile>(serial, false);
            INetworkClient m_Network = ServiceRegistry.GetService<INetworkClient>();
            m_Network.Send(new MobileQueryPacket(MobileQueryPacket.StatusType.BasicStatus, serial));
        }
    }
}
