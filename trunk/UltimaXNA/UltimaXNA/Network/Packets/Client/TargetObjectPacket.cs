﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Client
{
    public class TargetObjectPacket : SendPacket
    {
        public TargetObjectPacket(Serial serial)
            : base(0x6C, "Target Object", 19)
        {
            Stream.Write((byte)0x00); // BYTE[1] type: 0x00 = Select Object; 0x01 = Select X, Y, Z
            Stream.Write((int)0x00); // BYTE[4] cursorID 
            Stream.Write((byte)0x00); // BYTE[1] Cursor Type; 3 to cancel.
            Stream.Write((int)serial); // BYTE[4] Clicked On ID. Not used in this packet.
            Stream.Write((short)0); // BYTE[2] click xLoc
            Stream.Write((short)0); // BYTE[2] click yLoc
            Stream.Write((byte)0x00); // BYTE unknown (0x00)
            Stream.Write((byte)0x00); // BYTE click zLoc
            Stream.Write((short)0); // BYTE[2] model # (if a static tile, 0 if a map/landscape tile)
        }
    }
}
