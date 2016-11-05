using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.World.Entities.Mobiles;

namespace UltimaXNA.Ultima.Player.Partying
{
    public class PartyMember
    {
        public readonly Serial Serial;
        public Mobile Mobile => WorldModel.Entities.GetObject<Mobile>(Serial, false);

        public PartyMember(Serial serial)
        {
            Serial = serial;
        }
    }
}
