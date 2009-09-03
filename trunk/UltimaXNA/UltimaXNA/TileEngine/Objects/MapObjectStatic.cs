/***************************************************************************
 *   MapObjectStatic.cs
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
    public class MapObjectStatic : MapObject
    {
        public MapObjectStatic(Data.StaticTile staticTile, int sortInfluence, Vector3 position)
            : base(position)
        {
            ItemID = staticTile.ID;
            Tiebreaker = sortInfluence;
            // Set threshold.
            Data.ItemData itemData = Data.TileData.ItemData[ItemID & 0x3FFF];
            int background = (itemData.Background) ? 0 : 1;
            Threshold = (itemData.Height == 0) ? background : background + 1;

        }

        public bool Ignored
        {
            get { return (ItemID <= 1); }
        }
    }
}