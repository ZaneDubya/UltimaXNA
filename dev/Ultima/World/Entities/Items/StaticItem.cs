/***************************************************************************
 *   StaticItem.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using UltimaXNA.Ultima.World.Maps;

namespace UltimaXNA.Ultima.World.Entities.Items
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
