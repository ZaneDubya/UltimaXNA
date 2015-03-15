using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.UltimaData;

namespace UltimaXNA.Entity
{
    public class StaticItem : Item
    {
        public StaticItem(int itemID)
            : base(Serial.Null)
        {
            ItemID = itemID;
        }
    }
}
