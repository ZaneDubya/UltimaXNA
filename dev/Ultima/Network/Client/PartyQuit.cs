using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Network.Packets;
using UltimaXNA.Ultima.World;

namespace UltimaXNA.Ultima.Network.Client
{
    public class PParty_Quit : SendPacket
    {
        public PParty_Quit() : base(0xbf, "Quit Party")
        {
            Stream.Write((short)6);
            Stream.Write((byte)2);
            Stream.Write(WorldModel.Entities.GetPlayerEntity().Serial);
        }
    }
}
