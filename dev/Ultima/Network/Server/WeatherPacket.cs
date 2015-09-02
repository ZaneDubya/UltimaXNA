/***************************************************************************
 *   WeatherPacket.cs
 *   Copyright (c) 2009 UltimaXNA Development Team
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

namespace UltimaXNA.Ultima.Network.Server
{
    public class WeatherPacket : RecvPacket
    {
        readonly byte m_weatherType;
        readonly byte m_effectId;
        readonly byte m_temperature;

        public byte WeatherType
        {
            get { return m_weatherType; }
        }

        public byte NumberOfWeatherEffectsOnScreen
        {
            get { return m_effectId; }        
        }

        public byte Temperature 
        {
            get { return m_temperature; }
        }

        public WeatherPacket(PacketReader reader)
            : base(0x65, "Set Weather")
        {
            m_weatherType = reader.ReadByte();
            m_effectId = reader.ReadByte();
            m_temperature = reader.ReadByte();
        }
    }
}
