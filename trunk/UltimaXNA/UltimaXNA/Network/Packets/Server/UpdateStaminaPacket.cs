using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class UpdateStaminaPacket : RecvPacket
    {
        readonly Serial _serial;
        readonly short _current;
        readonly short _max;

        public Serial Serial
        {
            get { return _serial; }
        }

        public short Current
        {
            get { return _current; }
        }

        public short Max
        {
            get { return _max; } 
        }
        
        public UpdateStaminaPacket(PacketReader reader)
            : base(0xA3, "Update Stamina")
        {
            _serial = reader.ReadInt32();
            _max = reader.ReadInt16();
            _current = reader.ReadInt16();
        }
    }
}
