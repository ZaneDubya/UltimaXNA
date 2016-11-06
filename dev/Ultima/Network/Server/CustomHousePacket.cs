/***************************************************************************
 *   CustomHousePacket.cs
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
using UltimaXNA.Ultima.World.Data;
#endregion

namespace UltimaXNA.Ultima.Network.Server {
    public class CustomHousePacket : RecvPacket {
        readonly Serial m_HouseSerial;
        public Serial HouseSerial => m_HouseSerial;

        readonly int m_RevisionHash;
        public int RevisionHash => m_RevisionHash;

        readonly int m_NumPlanes;
        public int PlaneCount => m_NumPlanes;

        CustomHousePlane[] m_Planes;
        public CustomHousePlane[] Planes => m_Planes;

        public CustomHousePacket(PacketReader reader)
            : base(0xD8, "Custom House Packet") {
            byte CompressionType = reader.ReadByte();
            if (CompressionType != 3) {
                m_HouseSerial = Serial.Null;
                return;
            }
            reader.ReadByte(); // unknown, always 0?
            m_HouseSerial = reader.ReadInt32();
            m_RevisionHash = reader.ReadInt32();
            // this is for compression type 3 only
            int bufferLength = reader.ReadInt16();
            int trueBufferLength = reader.ReadInt16();
            m_NumPlanes = reader.ReadByte();
            // end compression type 3
            m_Planes = new CustomHousePlane[m_NumPlanes];
            for (int i = 0; i < m_NumPlanes; i++) {
                m_Planes[i] = new CustomHousePlane(reader);
            }
        }
    }
}
