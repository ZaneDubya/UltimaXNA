/***************************************************************************
 *   MapObjectComparer.cs
 *   Based on code from ClintXNA's renderer.
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
#endregion

namespace UltimaXNA.Entity.EntityViews
{
    class ViewSorter : IComparer<BaseEntity>
    {
        public static readonly ViewSorter Comparer = new ViewSorter();

        public int Compare(BaseEntity x, BaseEntity y)
        {
            int result = InternalGetSortZ(x) - InternalGetSortZ(y);

            if (result == 0)
                result = typeSortValue(x) - typeSortValue(y);

            return result;
        }

        private int InternalGetSortZ(BaseEntity entity)
        {
            int sort = entity.GetView().SortZ;
            if (entity is Ground)
                sort--;
            else if (entity is Item)
            {
                UltimaData.ItemData itemdata = ((Item)entity).ItemData;
                if (!itemdata.IsBackground)
                    sort++;
                if (!(itemdata.Height == 0))
                    sort++;
                if (itemdata.IsSurface)
                    sort--;
            }
            else if (entity is Mobile)
            {
                sort++;
            }
            return sort;
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
