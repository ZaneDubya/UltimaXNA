using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network
{
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
