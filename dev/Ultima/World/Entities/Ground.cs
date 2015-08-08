/***************************************************************************
 *   Ground.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.World.EntityViews;
using UltimaXNA.Ultima.World.Maps;
#endregion

namespace UltimaXNA.Ultima.World.Entities
{
    public class Ground : AEntity
    {
        // !!! Don't forget to update surrounding Z values - code is in UpdateSurroundingsIfNecessary(map)

        private int m_LandDataID;
        public int LandDataID
        {
            get { return m_LandDataID; }
        }

        public LandData LandData;

        public bool IsIgnored
        {
            get
            {
                return 
                    m_LandDataID == 2 ||
                    m_LandDataID == 0x1DB ||
                    (m_LandDataID >= 0x1AE && m_LandDataID <= 0x1B5);
            }
        }

        public Ground(int tileID, Map map)
            : base(Serial.Null, map)
        {
            m_LandDataID = tileID;
            LandData = TileData.LandData[m_LandDataID & 0x3FFF];
        }

        protected override AEntityView CreateView()
        {
            return new GroundView(this);
        }
    }
}
