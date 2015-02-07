/***************************************************************************
 *   WornItemPacket.cs
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
using UltimaXNA.Network;
#endregion

namespace UltimaXNA.Network.Packets.Server
{

    public class WornItemPacket : RecvPacket
    {
        readonly Serial _serial;
        readonly short _itemId;
        readonly byte _layer;
        readonly Serial _parentSerial;
        readonly short _hue;

        public Serial Serial
        {
            get { return _serial; }
        }

        public short ItemId
        {
            get { return _itemId; }
        }

        public byte Layer
        {
            get { return _layer; }
        }

        public Serial ParentSerial
        {
            get { return _parentSerial; }
        }

        public short Hue
        {
            get { return _hue; }
        }


        public WornItemPacket(PacketReader reader)
            : base(0x2E, "Worn Item")
        {
            _serial = reader.ReadInt32();
            _itemId = reader.ReadInt16();
            reader.ReadByte();
            _layer = reader.ReadByte();
            _parentSerial = reader.ReadInt32();
            _hue = reader.ReadInt16();
        }
    }
}
