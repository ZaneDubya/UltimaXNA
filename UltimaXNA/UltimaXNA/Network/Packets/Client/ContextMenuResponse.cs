using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class ContextMenuResponsePacket : SendPacket
    {
        public ContextMenuResponsePacket(Serial serial, short responseIndex)
            : base(0xBF, "Context Menu Response")
        {
            Stream.Write((short)0x15); // subcommand 0x15,  response to context menu
            Stream.Write((int)serial);
            Stream.Write((short)responseIndex);
        }
    }
}
