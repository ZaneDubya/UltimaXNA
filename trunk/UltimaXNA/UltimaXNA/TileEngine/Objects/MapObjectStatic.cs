﻿/***************************************************************************
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
        public MapObjectStatic(int staticTileID, int sortInfluence, Position3D position)
            : base(position)
        {
            ItemID = staticTileID;
            Tiebreaker = sortInfluence;
            // Set threshold.
            Data.ItemData itemData = Data.TileData.ItemData[ItemID & 0x3FFF];
            int background = (itemData.Background) ? 0 : 1;
            if (!itemData.Background)
                Threshold++;
            if (!(itemData.Height == 0))
                Threshold++;
            if (itemData.Surface)
                Threshold--;
            if (itemData.Name == "nodraw" || ItemID <= 0)
                _noDraw = true;
        }

        bool _noDraw = false;
        public bool NoDraw
        {
            get { return (_noDraw); }
        }

        public override string ToString()
        {
            return string.Format("Z:{0}   SortZ:{1}", Z, SortZ);
        }
    }
}