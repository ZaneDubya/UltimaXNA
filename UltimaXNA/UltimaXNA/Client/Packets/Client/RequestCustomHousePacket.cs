using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Client.Packets.Client
{
    public class RequestCustomHousePacket : SendPacket
    {
        public RequestCustomHousePacket(Serial serial)
            : base(0xBF, "Request Custom House Packet")
        {
            Stream.Write((short)0x1E); // subcommand 0x1E, request custom house
            Stream.Write((int)serial);
        }
    }
}
