using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
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
    }
}
