/***************************************************************************
 *   CustomHousePacket.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : February 24, 2010
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
using UltimaXNA.UltimaData;
using UltimaXNA.Network;
#endregion

namespace UltimaXNA.Network.Packets.Server
{
    public class CustomHousePacket : RecvPacket
    {
        readonly Serial _houseSerial;
        public Serial HouseSerial { get { return _houseSerial; } }

        readonly int _revisionHash;
        public int RevisionHash { get { return _revisionHash; } }

        readonly int _numPlanes;
        public int PlaneCount { get { return _numPlanes; } }

        CustomHousePlane[] _planes;
        public CustomHousePlane[] Planes { get { return _planes; } }

        public CustomHousePacket(PacketReader reader)
            : base(0xD8, "Custom House Packet")
        {
            byte CompressionType = reader.ReadByte();
            if (CompressionType != 3)
            {
                _houseSerial = Serial.Null;
                return;
            }
            reader.ReadByte(); // unknown, always 0?
            _houseSerial = reader.ReadInt32();
            _revisionHash = reader.ReadInt32();

            // this is for compression type 3 only
            int bufferLength = reader.ReadInt16();
            int trueBufferLength = reader.ReadInt16();
            _numPlanes = reader.ReadByte();
            // end compression type 3

            _planes = new CustomHousePlane[_numPlanes];
            for (int i = 0; i < _numPlanes; i++)
            {
                _planes[i] = new CustomHousePlane(reader);
            }
        }
    }
}
