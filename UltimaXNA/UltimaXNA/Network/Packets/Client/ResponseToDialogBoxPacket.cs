using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class ResponseToDialogBoxPacket : SendPacket
    {
        public ResponseToDialogBoxPacket(int dialogId, short menuId, short index, short modelNum, short color)
            : base(0x7D, "Response To Dialog Box", 13)
        {
            Stream.Write(dialogId);
            Stream.Write((short)menuId);
            Stream.Write((short)index);
            Stream.Write((short)modelNum);
            Stream.Write((short)color);
        }
    }
}
