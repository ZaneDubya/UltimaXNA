using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Network;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Ultima.World.Entities.Mobiles;

namespace UltimaXNA.Ultima.World.Data {
    public class PartyMember {
        public PartyMember(Serial _serial, bool _isleader) {
            isLeader = _isleader;
            Player = WorldModel.Entities.GetObject<Mobile>(_serial, false);
            INetworkClient m_Network = ServiceRegistry.GetService<INetworkClient>();
            m_Network.Send(new PartyQueryStats(_serial));//I THINK CHECK FOR STATUS ??
        }

        public bool isLeader { get; set; }

        public bool isLootable { get; set; }   //only working on client Entity

        public Mobile Player { get; set; }
    }
}
