/***************************************************************************
 *   TargetXYZPacket.cs
 *   Copyright (c) 2009 UltimaXNA Development Team
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.Ultima.Network.Client
{
    public class TargetXYZPacket : SendPacket
    {
        public TargetXYZPacket(short x, short y, short z, ushort modelNumber, int cursorID, byte targetType)
            : base(0x6C, "Target XYZ", 19)
        {
            Stream.Write((byte)0x01); // BYTE[1] type: 0x00 = Select Object; 0x01 = Select X, Y, Z
            Stream.Write(cursorID); // BYTE[4] cursorID 
            Stream.Write(targetType); // BYTE[1] Cursor Type; 3 to cancel.
            Stream.Write((int)0x00); // BYTE[4] Clicked On ID. Not used in this packet.
            Stream.Write(x); // BYTE[2] click xLoc
            Stream.Write(y); // BYTE[2] click yLoc
            Stream.Write(z); // BYTE click zLoc
            Stream.Write(modelNumber); // BYTE[2] model # (if a static tile, 0 if a map/landscape tile)
        }
    }
}
