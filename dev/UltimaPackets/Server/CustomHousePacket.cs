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
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.UltimaPackets.Server
{
    public class CustomHousePacket : RecvPacket
    {
        readonly Serial m_houseSerial;
        public Serial HouseSerial { get { return m_houseSerial; } }

        readonly int m_revisionHash;
        public int RevisionHash { get { return m_revisionHash; } }

        readonly int m_numPlanes;
        public int PlaneCount { get { return m_numPlanes; } }

        CustomHousePlane[] m_planes;
        public CustomHousePlane[] Planes { get { return m_planes; } }

        public CustomHousePacket(PacketReader reader)
            : base(0xD8, "Custom House Packet")
        {
            byte CompressionType = reader.ReadByte();
            if (CompressionType != 3)
            {
                m_houseSerial = Serial.Null;
                return;
            }
            reader.ReadByte(); // unknown, always 0?
            m_houseSerial = reader.ReadInt32();
            m_revisionHash = reader.ReadInt32();

            // this is for compression type 3 only
            int bufferLength = reader.ReadInt16();
            int trueBufferLength = reader.ReadInt16();
            m_numPlanes = reader.ReadByte();
            // end compression type 3

            m_planes = new CustomHousePlane[m_numPlanes];
            for (int i = 0; i < m_numPlanes; i++)
            {
                m_planes[i] = new CustomHousePlane(reader);
            }
        }
    }
}
