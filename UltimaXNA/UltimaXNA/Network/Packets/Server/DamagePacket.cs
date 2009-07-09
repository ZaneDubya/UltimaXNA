using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class DamagePacket : RecvPacket
    {
        readonly Serial _serial;
        readonly short _damage;

        public Serial Serial
        {
            get { return _serial; }
        } 

        public short Damage
        {
            get { return _damage; }
        } 
        
        public DamagePacket(PacketReader reader)
            : base(0x0B, "Damage")
        {
            _serial = reader.ReadInt32();
            _damage = reader.ReadInt16();
        }
    }
}
