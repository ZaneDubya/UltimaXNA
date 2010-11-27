/***************************************************************************
 *   StartingLocation.cs
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

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
