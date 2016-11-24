/***************************************************************************
 *   QueuedPacket.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

namespace UltimaXNA.Core.Network
{
    class QueuedPacket
    {
        public PacketHandler PacketHandler;
        public byte[] PacketBuffer;
        public int RealLength;

        public QueuedPacket(PacketHandler packetHandler, byte[] packetBuffer, int realLength)
        {
            PacketHandler = packetHandler;
            PacketBuffer = packetBuffer;
            RealLength = realLength;
        }
    }
}
