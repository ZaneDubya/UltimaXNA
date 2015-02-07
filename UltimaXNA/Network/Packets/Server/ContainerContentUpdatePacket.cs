/***************************************************************************
 *   ContainerContentUpdatePacket.cs
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
    public class ContainerContentUpdatePacket : RecvPacket
    {
        readonly Serial _serial;
        readonly int _itemId;
        readonly int _amount;
        readonly int _x;
        readonly int _y;
        readonly int _gridLocation;
        readonly Serial _parentSerial;
        readonly int _hue;

        public int ItemId
        {
            get { return _itemId; }
        }

        public int Amount
        {
            get { return _amount; }
        }

        public int X
        {
            get { return _x; }
        }

        public int Y
        {
            get { return _y; }
        }

        public int GridLocation
        {
            get { return _gridLocation; }
        }

        public Serial ContainerSerial
        {
            get { return _parentSerial; }
        }

        public int Hue
        {
            get { return _hue; }
        }

        public Serial Serial
        {
            get { return _serial; }
        }

        public ContainerContentUpdatePacket(PacketReader reader)
            : base(0x25, "Add Single Item")
        {
            _serial = reader.ReadInt32();
            _itemId = reader.ReadUInt16();
            reader.ReadByte(); // unknown 
            _amount = reader.ReadUInt16();
            _x = reader.ReadInt16();
            _y = reader.ReadInt16();
            _gridLocation = reader.ReadByte(); // always 0 in RunUO.
            _parentSerial = (Serial)reader.ReadInt32();
            _hue = reader.ReadUInt16();
        }
    }
}
