/***************************************************************************
 *   CustomHousing.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System.Collections.Generic;

namespace UltimaXNA.Ultima.World.Data {
    class CustomHousing {
        static readonly Dictionary<Serial, CustomHouse> m_CustomHouses = new Dictionary<Serial, CustomHouse>();

        public static bool IsHashCurrent(Serial serial, int hash) {
            if (m_CustomHouses.ContainsKey(serial)) {
                CustomHouse h = m_CustomHouses[serial];
                return (h.Hash == hash);
            }
            return false;
        }

        public static CustomHouse GetCustomHouseData(Serial serial) => m_CustomHouses[serial];

        public static void UpdateCustomHouseData(Serial serial, int hash, int planecount, CustomHousePlane[] planes) {
            CustomHouse house;
            if (m_CustomHouses.ContainsKey(serial)) {
                house = m_CustomHouses[serial];
            }
            else {
                house = new CustomHouse(serial);
                m_CustomHouses.Add(serial, house);
            }
            house.Update(hash, planecount, planes);
        }
    }
}