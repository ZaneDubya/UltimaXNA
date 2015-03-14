using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.UltimaData;

namespace UltimaXNA.Entity
{
    public class StaticItem : BaseEntity
    {
        public int ItemID;
        public ItemData ItemData;

        public StaticItem(int itemID)
            : base(Serial.Null)
        {
            ItemID = itemID;
            ItemData = UltimaData.TileData.ItemData[ItemID & 0x3FFF];
        }
    }
}
