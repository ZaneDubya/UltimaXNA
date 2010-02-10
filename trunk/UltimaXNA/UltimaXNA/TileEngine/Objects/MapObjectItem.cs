/***************************************************************************
 *   MapObjectItem.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
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
using Microsoft.Xna.Framework;
using UltimaXNA.Entities;
#endregion

namespace UltimaXNA.TileEngine
{
    public class MapObjectItem : MapObject
    {
        public int Hue { get; set; }
        public int Facing { get; set; }

        public MapObjectItem(int itemID, Position3D position, int direction, Entity ownerEntity, int hue)
            : base(position)
        {
            ItemID = itemID;
            OwnerEntity = ownerEntity;
            Facing = direction;
            Hue = hue;
        }
    }
}
