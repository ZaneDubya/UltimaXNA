/***************************************************************************
 *   SubServerPacket.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.UltimaPackets.Server
{
    public class SubServerPacket : RecvPacket
    {
        readonly short _x;
        readonly short _y;
        readonly short _z;
        readonly short _mapWidth;
        readonly short _mapHeight;

        public short X
        {
            get { return _x; }
        }

        public short Y
        {
            get { return _y; }
        }

        public short Z
        {
            get { return _z; }
        }

        public short MapWidth
        {
            get { return _mapWidth; }
        }

        public short MapHeight
        {
            get { return _mapHeight; }
        }

        public SubServerPacket(PacketReader reader)
            : base(0xB3, "Chat Packet")
        {
            _x = reader.ReadInt16();
            _y = reader.ReadInt16();
            _z = reader.ReadInt16();
            reader.ReadByte();
            reader.ReadInt16();
            reader.ReadInt16();
            _mapWidth = reader.ReadInt16();
            _mapHeight = reader.ReadInt16();
        }
    }
}
