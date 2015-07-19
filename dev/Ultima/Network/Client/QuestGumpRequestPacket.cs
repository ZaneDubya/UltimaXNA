/***************************************************************************
 *   QuestGumpRequestPacket.cs
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
    public class QuestGumpRequestPacket : SendPacket
    {
        public QuestGumpRequestPacket(Serial serial)
            : base(0xD7, "Quest gump request")
        {
            Stream.Write(serial);
            Stream.Write((ushort)0x0032);
            Stream.Write((byte)0x00);
        }
    }
}
