/***************************************************************************
 *   EntitySort.cs
 *   Based on code from ClintXNA & PlayUO.
 *   
 *   Liiiicense????
 * 
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Entities.Effects;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Entities.Mobiles;
#endregion

namespace UltimaXNA.Ultima.World.Maps
{
    static class TileSorter
    {
        public static void Sort(List<AEntity> items)
        {
            for (int i = 0; i < items.Count - 1; i++)
            {
                int j = i + 1;

                while (j > 0)
                {
                    int result = Compare(items[j - 1], items[j]);
                    if (result > 0)
                    {
                        AEntity temp = items[j - 1];
                        items[j - 1] = items[j];
                        items[j] = temp;

                    }
                    j--;
                }
            }
        }

        public static int Compare(AEntity x, AEntity y)
        {
            int xZ;
            int xThreshold;
            int xType;
            int xTiebreaker;
            int yZ;
            int yThreshold;
            int yType;
            int yTiebreaker;

            GetSortValues(x, out xZ, out xThreshold, out xType, out xTiebreaker);
            GetSortValues(y, out yZ, out yThreshold, out yType, out yTiebreaker);

            xZ += xThreshold;
            yZ += yThreshold;

            int comparison = xZ - yZ;
            if (comparison == 0)
            {
                comparison = xType - yType;
            }
            if (comparison == 0)
            {
                comparison = xThreshold - yThreshold;
            }
            if (comparison == 0)
            {
                comparison = xTiebreaker - yTiebreaker;
            }
            return comparison;
        }

        public static void GetSortValues(AEntity e, out int z, out int threshold, out int type, out int tiebreaker)
        {
            if (e is AEffect)
            {
                AEffect effect = e as AEffect;
                z = effect.Z;
                threshold = 2;
                type = 4;
                tiebreaker = 0;
            }
            else if (e is DeferredEntity)
            {
                DeferredEntity mobile = (DeferredEntity)e;
                z = mobile.Z;
                threshold = 1;
                type = 2;
                tiebreaker = 0;
            }
            else if (e is Mobile)
            {
                Mobile mobile = (Mobile)e;
                z = mobile.Z;
                threshold = 2;
                type = 3;
                if (mobile.IsClientEntity)
                {
                    tiebreaker = 0x40000000;
                }
                else
                {
                    tiebreaker = mobile.Serial;
                }
            }
            else if (e is Ground)
            {
                Ground tile = (Ground)e;
                z = tile.GetView().SortZ;
                threshold = 0;
                type = 0;
                tiebreaker = 0;
            }
            else if (e is StaticItem)
            {
                int sort;
                StaticItem item = (StaticItem)e;
                z = item.Z;
                if (item.ItemData.IsBackground)
                {
                    sort = 0;
                }
                else
                {
                    sort = 1;
                }
                threshold = (item.ItemData.Height == 0) ? sort : (sort + 1);
                type = 1;
                tiebreaker = item.SortInfluence;
            }
            else if (e is Item) // this was previously dynamicitem - I think Kirros and I use the word 'Dynamic' for different purposes.
            {
                int sort;
                Item item = (Item)e;
                z = item.Z;
                if (item.ItemData.IsBackground)
                {
                    sort = 0;
                }
                else
                {
                    sort = 1;
                }
                threshold = (item.ItemData.Height == 0) ? sort : (sort + 1);
                type = ((item.ItemID & 0x3fff) == 0x2006) ? 4 : 2; // is corpse? Corpse inherits from Container, which inherits from Item, so this works here.
                tiebreaker = item.Serial;
            }
            else
            {
                z = 0;
                threshold = 0;
                type = 0;
                tiebreaker = 0;
            }
        }
    }
}
