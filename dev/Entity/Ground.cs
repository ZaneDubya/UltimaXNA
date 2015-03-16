using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.UltimaData;

namespace UltimaXNA.Entity
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

        public Ground(int tileID)
            : base(Serial.Null)
        {
            m_LandDataID = tileID;
            LandData = UltimaData.TileData.LandData[m_LandDataID & 0x3FFF];
        }

        protected override EntityViews.AEntityView CreateView()
        {
            return new EntityViews.GroundView(this);
        }
    }
}
