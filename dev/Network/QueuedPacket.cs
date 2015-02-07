/***************************************************************************
 *   QueuedPacket.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace UltimaXNA.Network
{
    class QueuedPacket
    {
        public List<PacketHandler> PacketHandlers;
        public byte[] PacketBuffer;
        public int RealLength;
        public string Name;

        public QueuedPacket(string name, List<PacketHandler> packetHandlers, byte[] packetBuffer, int realLength)
        {
            Name = name;
            PacketHandlers = packetHandlers;
            PacketBuffer = packetBuffer;
            RealLength = realLength;
        }
    }
}
