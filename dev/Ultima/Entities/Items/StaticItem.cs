using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Ultima.IO;
using UltimaXNA.Ultima.World.Maps;

namespace UltimaXNA.Ultima.Entities.Items
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
