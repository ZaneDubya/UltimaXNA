/***************************************************************************
 *   CharacterCityListPacket.cs
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
using UltimaXNA.UltimaVars;
#endregion

namespace UltimaXNA.UltimaPackets.Server
{
    public class CharacterCityListPacket : RecvPacket
    {
        readonly StartingLocation[] _locations;
        readonly CharacterListEntry[] _characters;

        public StartingLocation[] Locations
        {
            get { return _locations; }
        }

        public CharacterListEntry[] Characters
        {
            get { return _characters; }
        }

        public CharacterCityListPacket(PacketReader reader)
            : base(0xA9, "Char/City List")
        {
            int characterCount = reader.ReadByte();
            _characters = new CharacterListEntry[characterCount];

            for (int i = 0; i < characterCount; i++)
            {
                _characters[i] = new CharacterListEntry(reader);
            }

            int locationCount = reader.ReadByte();
            _locations = new StartingLocation[locationCount];

            for (int i = 0; i < locationCount; i++)
            {
                _locations[i] = new StartingLocation(reader);
            }
        }

        public class StartingLocation
        {
            readonly byte index;
            readonly string cityName;
            readonly string areaOfCityOrTown;

            public byte Index
            {
                get { return index; }
            }

            public string CityName
            {
                get { return cityName; }
            }

            public string AreaOfCityOrTown
            {
                get { return areaOfCityOrTown; }
            }

            public StartingLocation(PacketReader reader)
            {
                this.index = reader.ReadByte();
                this.cityName = reader.ReadString(31);
                this.areaOfCityOrTown = reader.ReadString(31);
            }
        }
    }
}
