using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class RequestNameResponsePacket : RecvPacket
    {
        readonly Serial _serial;
        readonly string _mobileName;

        public Serial Serial
        { 
            get { return _serial; } 
        }

        public string MobileName
        {
            get { return _mobileName; }
        }

        public RequestNameResponsePacket(PacketReader reader)
            : base(0x98, "Request Name Response")
        {
            _serial = reader.ReadInt32();
            _mobileName = reader.ReadString(30);
        }
    }
}
