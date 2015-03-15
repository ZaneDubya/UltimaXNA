/***************************************************************************
 *   MapObjectComparer.cs
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using UltimaXNA.Entity;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.UltimaWorld.View
{
    sealed class MapObjectComparer : IComparer<BaseEntity>
    {
        public static readonly MapObjectComparer Comparer = new MapObjectComparer();

        public int Compare(BaseEntity x, BaseEntity y)
        {
            int result = InternalGetSortZ(x) - InternalGetSortZ(y);

            if (result == 0)
                result = typeSortValue(x) - typeSortValue(y);

            return result;
        }

        private int InternalGetSortZ(BaseEntity entity)
        {
            int sort = entity.Z;
            if (entity is Ground)
                sort--;
            else if (entity is StaticItem)
            {
                UltimaData.ItemData itemdata = ((StaticItem)entity).ItemData;
                if (!itemdata.IsBackground)
                    sort++;
                if (!(itemdata.Height == 0))
                    sort++;
                if (itemdata.IsSurface)
                    sort--;
            }
            return entity.Z;
        }

        private int typeSortValue(BaseEntity mapobject)
        {
            if (mapobject is Ground)
                return 0;
            else if (mapobject is StaticItem)
                return 1;
            else if (mapobject is Item)
                return 2;
            else if (mapobject is Mobile)
                return 3;
            //else if (type == typeof(MapObjectText))
            //    return 4;
            //else if (type == typeof(MapObjectDynamic))
            //    return 5;
            return -100;
        }
    }
}
