/***************************************************************************
 *   ContainerPacket.cs
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
    public class OpenContainerPacket : RecvPacket
    {
        readonly Serial _serial;
        readonly ushort _gumpId;

        public Serial Serial { get { return _serial; } }
        public ushort GumpId { get { return _gumpId; } }

        public OpenContainerPacket(PacketReader reader)
            : base(0x24, "Open Container")
        {
            this._serial = reader.ReadInt32();
            this._gumpId = reader.ReadUInt16();
        }
    }
}
