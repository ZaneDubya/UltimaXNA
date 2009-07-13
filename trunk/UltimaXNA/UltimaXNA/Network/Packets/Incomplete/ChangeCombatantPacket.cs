using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class ChangeCombatantPacket : RecvPacket
    {
        public readonly Serial Serial;
        public ChangeCombatantPacket(PacketReader reader)
            : base(0xAA, "Change Combatant")
        {
            Serial = reader.ReadInt32();
        }
    }
}
