using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.UltimaData;
using UltimaXNA.UltimaWorld.Maps;

namespace UltimaXNA.UltimaEntities
{
    public class StaticItem : Item
    {
        public int SortInfluence = 0;

        public StaticItem(int itemID, int hue,  int sortInfluence, Map map)
            : base(Serial.Null, map)
        {
            ItemID = itemID;
            Hue = hue;
            SortInfluence = sortInfluence;
        }
    }
}
