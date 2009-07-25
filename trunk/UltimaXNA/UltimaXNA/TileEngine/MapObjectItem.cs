/***************************************************************************
 *   GameObjectTile.cs
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
        public int Hue { get; internal set; }
        public int Facing { get; internal set; }

        public MapObjectItem(int itemID, Vector3 position, int direction, Entity ownerEntity, int hue)
            : base(new Vector2(position.X, position.Y))
        {
            ItemID = itemID;
            OwnerEntity = ownerEntity;
            Facing = direction;
            Hue = hue;
            Z = (int)position.Z;
        }

        public new int SortZ
        {
            get { return Z; }
        }
    }
}
