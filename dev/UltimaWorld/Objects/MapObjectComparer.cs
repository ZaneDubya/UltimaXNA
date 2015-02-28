/***************************************************************************
 *   MapObjectComparer.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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

namespace UltimaXNA.UltimaWorld
{
    sealed class MapObjectComparer : IComparer<AMapObject>
    {
        public static readonly MapObjectComparer Comparer = new MapObjectComparer();

        public int Compare(AMapObject x, AMapObject y)
        {
            int result = (x.SortZ + x.SortThreshold) - (y.SortZ + y.SortThreshold);

            if (result == 0)
                result = x.SortThreshold - y.SortThreshold;

            if (result == 0)
                result = x.SortTiebreaker - y.SortTiebreaker;

            if (result == 0)
                result = typeSortValue(x) - typeSortValue(y);

            return result;
        }

        private int typeSortValue(AMapObject mapobject)
        {
            Type type = mapobject.GetType();
            if (type == typeof(MapObjectGround))
                return 0;
            else if (type == typeof(MapObjectStatic))
                return 1;
            else if (type == typeof(MapObjectItem))
                return 2;
            else if (type == typeof(MapObjectMobile))
                return 3;
            else if (type == typeof(MapObjectText))
                return 4;
            else if (type == typeof(MapObjectDynamic))
                return 5;
            return -100;
        }
    }
}
