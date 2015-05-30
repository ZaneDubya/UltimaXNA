using System.Collections.Generic;
using System.IO;
using System.Linq;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Patterns;
using UltimaXNA.Ultima.IO;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Ultima.Entities;
using UltimaXNA.Ultima.Entities.Items;
using UltimaXNA.Ultima.Data;

namespace SeasonChangerPlugin
{
    internal sealed class SeasonChangerModule : IModule
    {
        public string Name
        {
            get { return "Season Changer Plugin"; }
        }

        public void Load()
        {
            m_TranslationTable = CreateTranslationTable();
            Map.SeasonalTranslator += TranslateMapBlock;
        }

        public void Unload()
        {
            Map.SeasonalTranslator -= TranslateMapBlock;
            m_TranslationTable = null;
        }

        private void TranslateMapBlock(MapBlock block, Seasons season)
        {
            if (block == null)
                return;

            for (int tile = 0; tile < 64; tile++)
            {
                foreach (AEntity e in block.Tiles[tile].Entities)
                {
                    int[] translations;
                    if (e is StaticItem && m_TranslationTable.TryGetValue((e as StaticItem).ItemID, out translations))
                        (e as StaticItem).DisplayItemID = (season == Seasons.Spring) ? (e as StaticItem).ItemID : translations[(int)season - 1];
                }
            }
        }

        private Dictionary<int, int[]> m_TranslationTable; // Format of int[] is [summer, fall, winter, desolation]

        private Dictionary<int, int[]> CreateTranslationTable()
        {
            Dictionary<int, int[]> table = new Dictionary<int, int[]>();
            // Comment Format: ItemID,Map0 Count ; ItemData.Name (translation comment)
            table.Add(3247, new int[] { 3247, 3247, 3247, 1 }); // 3247,78640 ; grasses (not drawn in desolation)
            table.Add(3248, new int[] { 3248, 3248, 3248, 1 }); // 3248,64942 ; grasses
            table.Add(3253, new int[] { 3253, 3253, 3253, 1 }); // 3253,51766 ; grasses
            table.Add(3254, new int[] { 3254, 3254, 3254, 1 }); // 3254,51529 ; grasses
            table.Add(3244, new int[] { 3244, 3244, 3244, 1 }); // 3244,15517 ; grasses
            table.Add(3245, new int[] { 3245, 3245, 3245, 1 }); // 3245,15014 ; grasses
            // 3286,14834 ; cedar tree
            table.Add(3287, new int[] { 3287, 3287, 3287, 1 }); // 3287,14822 ; cedar needles (not drawn in desolation)
            // 3288,14730 ; cedar tree
            table.Add(3289, new int[] { 3289, 3289, 3289, 1 }); // 3289,14719 ; cedar needles
            table.Add(3250, new int[] { 3250, 3250, 3250, 1 }); // 3250,14016 ; grasses
            table.Add(3251, new int[] { 3251, 3251, 3251, 1 }); // 3251,13976 ; grasses
            table.Add(3246, new int[] { 3246, 3246, 3246, 1 }); // 3246,13975 ; grasses
            table.Add(3249, new int[] { 3249, 3249, 3249, 1 }); // 3249,13960 ; grasses
            table.Add(3252, new int[] { 3252, 3252, 3252, 1 }); // 3252,13923 ; grasses
            // 3280,13269 ; tree
            table.Add(3281, new int[] { 3281, 3282, 1, 1 }); // 3281,13234 ; leaves (fall color, not drawn in win/des)
            // 3283,13228 ; tree
            // 3290,13225 ; oak tree
            table.Add(3284, new int[] { 3284, 3285, 1, 1 }); // 3284,13218 ; leaves (fall color, not drawn in win/des)
            table.Add(3291, new int[] { 3291, 3292, 1, 1 }); // 3291,13217 ; oak leaves (fall color, not drawn in win/des)
            // 3302,13119 ; willow tree
            // 3293,13097 ; oak tree
            table.Add(3303, new int[] { 3303, 3304, 1, 1 }); // 3303,13088 ; willow leaves (fall color, not drawn in win/des)
            table.Add(3294, new int[] { 3294, 3295, 1, 1 }); // 3294,13083 ; oak leaves (fall color, not drawn in win/des)
            // 3277,12938 ; tree
            table.Add(3278, new int[] { 3278, 3279, 1, 1 }); // 3278,12924 ; leaves (fall color, not drawn in win/des)
            // 3299,12851 ; walnut tree
            table.Add(3300, new int[] { 3300, 3301, 1, 1 }); // 3300,12838 ; walnut leaves (fall color, not drawn in win/des)
            // 3296,12780 ; walnut tree
            table.Add(3297, new int[] { 3297, 3298, 1, 1 }); // 3297,12768 ; walnut leaves (fall color, not drawn in win/des)
            table.Add(3306, new int[] { 3306, 3391, 3391, 3391 }); // 3306,10645 ; sapling (brambles in fal/win/des)
            // 3319,10615 ; fallen log
            table.Add(3305, new int[] { 3305, 3392, 3392, 3392 }); // 3305,10395 ; sapling (brambles in fal/win/des)
            // 3239,8144 ; rushes (despite these being common, I couldn't find any in the map.)
            table.Add(3375, new int[] { 3248, 1, 3248, 1 }); // 3375,8057 ; flowers (-> grasses -> nothing -> grasses -> nothing)
            table.Add(3373, new int[] { 3373, 3373, 3373, 1 }); // 3373,7858 ; flowers (not drawn in des)
            table.Add(3371, new int[] { 3371, 3371, 3371, 6933 }); // 3371,7634 ; flowers (pelvis in des)
            table.Add(3205, new int[] { 3205, 3205, 3205, 7068 }); // 3205,7425 ; orfluer flowers (stick in des)
            table.Add(3204, new int[] { 3204, 0, 0, 7044 }); // 3204,6577 ; foxglove flowers (nothing in fall/win;wood in des)
            table.Add(3211, new int[] { 3211, 3378, 0, 7044 }); // 3211,6315 ; white flowers (grasses in fall,disp in winter,wood in des)
            table.Add(3212, new int[] { 3212, 3379, 0, 7044 }); // 3212,6279 ; white flowers (grasses in fall,disp in winter,wood in des)
            // 3271,6268 ; weed
            // 3269,6262 ; grasses
            // 3415,5594 ; tree
            // 3350,4330 ; mushroom
            // 3442,3861 ; tree
            // 3439,3855 ; tree
            // 3438,3854 ; tree
            // 3440,3853 ; tree
            // 3441,3852 ; tree
            // 3393,3843 ; tree
            // 3396,3841 ; tree
            // 3395,3840 ; tree
            // 3394,3838 ; tree
            // 3353,3831 ; mushroom
            // 3462,3805 ; tree
            // 3460,3804 ; tree
            // 3461,3801 ; tree
            // 3351,3782 ; mushroom
            // 3419,3760 ; tree
            // 3416,3744 ; tree
            // 3417,3743 ; tree
            // 3418,3743 ; tree
            // 3349,3729 ; mushrooms
            // 3352,3715 ; mushroom
            // 3346,3548 ; mushrooms
            // 3348,3535 ; mushrooms
            // 3347,3528 ; mushrooms
            // 3344,3420 ; mushrooms
            // 3342,3400 ; mushrooms
            // 3341,3329 ; mushrooms
            // 3345,3315 ; mushrooms
            // 3340,3305 ; mushrooms
            // 3343,3208 ; mushrooms
            // 3219,2703 ; blade plant
            // 2273,2660 ; stalagmites
            // 3220,2495 ; bulrushes
            table.Add(3450, new int[] { 3450, 3450, 0, 0 }); // 3450,1980 ; leaves (tall trees, nothing in win/des)
            table.Add(3455, new int[] { 3455, 3455, 0, 0 }); // 3455,1977 ; leaves (tall trees, nothing in win/des)
            table.Add(3451, new int[] { 3451, 3451, 0, 0 }); // 3451,1977 ; leaves (tall trees, nothing in win/des)
            table.Add(3454, new int[] { 3454, 3454, 0, 0 }); // 3454,1975 ; leaves (tall trees, nothing in win/des)
            table.Add(3452, new int[] { 3452, 3452, 0, 0 }); // 3452,1975 ; leaves (tall trees, nothing in win/des)
            table.Add(3453, new int[] { 3453, 3453, 0, 0 }); // 3453,1975 ; leaves (tall trees, nothing in win/des)
            table.Add(3404, new int[] { 3404, 3404, 0, 0 }); // 3404,1924 ; leaves (tall trees, nothing in win/des)
            table.Add(3411, new int[] { 3411, 3411, 0, 0 }); // 3411,1924 ; leaves (tall trees, nothing in win/des)
            table.Add(3402, new int[] { 3402, 3402, 0, 0 }); // 3402,1918 ; leaves (tall trees, nothing in win/des)
            table.Add(3407, new int[] { 3407, 3407, 0, 0 }); // 3407,1917 ; leaves (tall trees, nothing in win/des)
            table.Add(3408, new int[] { 3408, 3408, 0, 0 }); // 3408,1917 ; leaves (tall trees, nothing in win/des)
            table.Add(3406, new int[] { 3406, 3406, 0, 0 }); // 3406,1917 ; leaves (tall trees, nothing in win/des)
            table.Add(3403, new int[] { 3403, 3403, 0, 0 }); // 3403,1916 ; leaves (tall trees, nothing in win/des)
            table.Add(3409, new int[] { 3409, 3409, 0, 0 }); // 3409,1916 ; leaves (tall trees, nothing in win/des)
            table.Add(3410, new int[] { 3410, 3410, 0, 0 }); // 3410,1915 ; leaves (tall trees, nothing in win/des)
            table.Add(3405, new int[] { 3405, 3405, 0, 0 }); // 3405,1915 ; leaves (tall trees, nothing in win/des)
            table.Add(3401, new int[] { 3401, 3401, 0, 0 }); // 3401,1914 ; leaves (tall trees, nothing in win/des)
            table.Add(3400, new int[] { 3400, 3400, 0, 0 }); // 3400,1912 ; leaves (tall trees, nothing in win/des)
            table.Add(3398, new int[] { 3398, 3398, 0, 0 }); // 3398,1910 ; leaves (tall trees, nothing in win/des)
            table.Add(3397, new int[] { 3397, 3397, 0, 0 }); // 3397,1910 ; leaves (tall trees, nothing in win/des)
            table.Add(3449, new int[] { 3449, 3449, 0, 0 }); // 3449,1909 ; leaves (tall trees, nothing in win/des)
            table.Add(3399, new int[] { 3399, 3399, 0, 0 }); // 3399,1909 ; leaves (tall trees, nothing in win/des)
            table.Add(3467, new int[] { 3467, 3467, 0, 0 }); // 3467,1907 ; leaves (tall trees, nothing in win/des)
            table.Add(3465, new int[] { 3465, 3465, 0, 0 }); // 3465,1905 ; leaves (tall trees, nothing in win/des)
            table.Add(3466, new int[] { 3466, 3466, 0, 0 }); // 3466,1905 ; leaves (tall trees, nothing in win/des)
            table.Add(3463, new int[] { 3463, 3463, 0, 0 }); // 3463,1905 ; leaves (tall trees, nothing in win/des)
            table.Add(3464, new int[] { 3464, 3464, 0, 0 }); // 3464,1904 ; leaves (tall trees, nothing in win/des)
            table.Add(3420, new int[] { 3420, 3420, 0, 0 }); // 3420,1896 ; leaves (tall trees, nothing in win/des)
            table.Add(3424, new int[] { 3424, 3424, 0, 0 }); // 3424,1896 ; leaves (tall trees, nothing in win/des)
            table.Add(3422, new int[] { 3422, 3422, 0, 0 }); // 3422,1895 ; leaves (tall trees, nothing in win/des)
            table.Add(3421, new int[] { 3421, 3421, 0, 0 }); // 3421,1895 ; leaves (tall trees, nothing in win/des)
            table.Add(3423, new int[] { 3423, 3423, 0, 0 }); // 3423,1895 ; leaves (tall trees, nothing in win/des)
            table.Add(3426, new int[] { 3426, 3426, 0, 0 }); // 3426,1894 ; leaves (tall trees, nothing in win/des)
            table.Add(3425, new int[] { 3425, 3425, 0, 0 }); // 3425,1894 ; leaves (tall trees, nothing in win/des)
            table.Add(3468, new int[] { 3468, 3468, 0, 0 }); // 3468,1892 ; leaves (tall trees, nothing in win/des)
            table.Add(3472, new int[] { 3472, 3472, 0, 0 }); // 3472,1891 ; leaves (tall trees, nothing in win/des)
            table.Add(3471, new int[] { 3471, 3471, 0, 0 }); // 3471,1890 ; leaves (tall trees, nothing in win/des)
            table.Add(3470, new int[] { 3470, 3470, 0, 0 }); // 3470,1890 ; leaves (tall trees, nothing in win/des)
            table.Add(3469, new int[] { 3469, 3469, 0, 0 }); // 3469,1889 ; leaves (tall trees, nothing in win/des)
            table.Add(3448, new int[] { 3448, 3448, 0, 0 }); // 3448,1869 ; leaves (tall trees, nothing in win/des)
            table.Add(3447, new int[] { 3447, 3447, 0, 0 }); // 3447,1868 ; leaves (tall trees, nothing in win/des)
            table.Add(3445, new int[] { 3445, 3445, 0, 0 }); // 3445,1868 ; leaves (tall trees, nothing in win/des)
            table.Add(3444, new int[] { 3444, 3444, 0, 0 }); // 3444,1868 ; leaves (tall trees, nothing in win/des)
            table.Add(3446, new int[] { 3446, 3446, 0, 0 }); // 3446,1867 ; leaves (tall trees, nothing in win/des)
            table.Add(3427, new int[] { 3427, 3427, 0, 0 }); // 3427,1847 ; leaves (tall trees, nothing in win/des)
            table.Add(3428, new int[] { 3428, 3428, 0, 0 }); // 3428,1844 ; leaves (tall trees, nothing in win/des)
            table.Add(3432, new int[] { 3432, 3432, 0, 0 }); // 3432,1843 ; leaves (tall trees, nothing in win/des)
            table.Add(3431, new int[] { 3431, 3431, 0, 0 }); // 3431,1843 ; leaves (tall trees, nothing in win/des)
            table.Add(3429, new int[] { 3429, 3429, 0, 0 }); // 3429,1843 ; leaves (tall trees, nothing in win/des)
            table.Add(3430, new int[] { 3430, 3430, 0, 0 }); // 3430,1842 ; leaves (tall trees, nothing in win/des)
            table.Add(3433, new int[] { 3433, 3433, 0, 0 }); // 3433,1841 ; leaves (tall trees, nothing in win/des)

            // 3333,1832 ; reeds
            // 3241,1795 ; snake plant
            // 2272,1701 ; stalagmites
            // 4150,1115 ; hay
            // 3332,1050 ; water plants
            // 3339,1042 ; lilypads
            // 4151,914 ; hay
            // 3337,910 ; lilypad
            // 3338,812 ; lilypad
            // 3892,793 ; hay
            // 3320,660 ; cypress tree
            // 3334,653 ; lilypad
            // 3335,638 ; lilypad
            // 3336,624 ; lilypad
            // 6089,620 ; snow
            // 3148,584 ; flowers
            // 3146,580 ; flowers
            // 3323,526 ; cypress tree
            // 3374,522 ; cactus
            // 6270,414 ; spilled flour
            // 3329,413 ; cypress tree
            // 3149,411 ; flowers
            // 3326,401 ; cypress tree
            // 6092,398 ; snow
            // 3144,395 ; flowers
            // 3127,394 ; flowers
            // 4334,392 ; garbage
            // 3145,386 ; flowers
            // 3150,373 ; flowers
            // 3372,365 ; cactus
            // 3141,357 ; flowers
            // 3368,353 ; cactus
            // 3147,349 ; flowers
            // 3143,327 ; flowers
            // 3142,324 ; flowers
            // 4335,260 ; garbage
            // 6935,214 ; rib cage
            // 3321,202 ; cypress leaves
            // 3206,194 ; red poppies
            // 3208,193 ; snowdrops
            // 4152,183 ; wood curls
            // 3166,179 ; vines
            // 3203,179 ; campion flowers
            // 3213,178 ; white poppies
            // 4336,169 ; garbage
            // 3265,166 ; orfluer flowers
            // 3209,166 ; campion flowers
            // 3893,165 ; hay
            // 3210,164 ; foxglove flowers
            // 3168,163 ; vines
            // 6936,155 ; rib cage
            // 4337,147 ; garbage
            // 3167,143 ; vines
            // 3214,135 ; snowdrops
            // 3263,114 ; poppies
            // 6091,106 ; snow
            // 6090,100 ; snow
            // 4791,98 ; Yew tree
            // 3262,97 ; poppies
            // 3158,97 ; wheat
            // 4792,95 ; Yew tree
            // 4790,95 ; Yew tree
            // 4794,94 ; Yew tree
            // 4793,94 ; Yew tree
            // 3382,93 ; flowers
            table.Add(4807, new int[] { 4807, 4807, 0, 0 }); // 4807,93 ; Yew tree !
            table.Add(4806, new int[] { 4806, 4806, 0, 0 }); // 4806,93 ; Yew tree !
            table.Add(4805, new int[] { 4805, 4805, 0, 0 }); // 4805,93 ; Yew tree !
            // 4795,93 ; Yew tree
            // 4796,93 ; Yew tree
            // 4797,93 ; Yew tree
            table.Add(4804, new int[] { 4804, 4804, 0, 0 }); // 4804,92 ; Yew tree !
            table.Add(4803, new int[] { 4803, 4803, 0, 0 }); // 4803,92 ; Yew tree !
            table.Add(4802, new int[] { 4802, 4802, 0, 0 }); // 4802,92 ; Yew tree !
            table.Add(4801, new int[] { 4801, 4801, 0, 0 }); // 4801,92 ; Yew tree !
            table.Add(4800, new int[] { 4800, 4800, 0, 0 }); // 4800,92 ; Yew tree !
            table.Add(4799, new int[] { 4799, 4799, 0, 0 }); // 4799,92 ; Yew tree !
            table.Add(4798, new int[] { 4798, 4798, 0, 0 }); // 4798,92 ; Yew tree !
            // 3324,92 ; cypress leaves
            // 3160,88 ; wheat
            // 8612,83 ; no draw
            // 3190,80 ; carrots
            // 6943,72 ; leaves
            // 3207,71 ; campion flowers
            // 3157,63 ; wheat
            // 4758,62 ; post
            // 6087,61 ; snow
            // 6081,59 ; snow
            // 3186,59 ; squash
            // 3164,54 ; watermelon%s%
            // 4012,52 ; fire pit
            // 3195,52 ; head%s% of cabbage
            // 6271,51 ; spilled flour
            // 2645,51 ; bedroll
            // 3544,50 ; small fish
            // 3543,50 ; small fish
            // 3178,47 ; pumpkin%s%
            // 3183,44 ; onions
            // 3545,44 ; small fish
            // 4338,41 ; pile of garbage
            // 6086,40 ; snow
            // 3542,40 ; small fish
            // 3196,39 ; head%s% of cabbage
            // 14826,38 ; tree
            // 6273,37 ; spilled flour
            // 6946,37 ; leaves
            // 14825,37 ; tree
            // 14824,37 ; tree
            // 6945,36 ; leaves
            // 14823,36 ; tree
            // 3179,36 ; pumpkin%s%
            // 6088,35 ; snow
            // 6272,34 ; spilled flour
            // 3180,34 ; pumpkin%s%
            // 5193,31 ; loose grain
            // 3270,29 ; grasses
            // 6944,28 ; leaves
            // 14775,28 ; tree
            // 14774,28 ; tree
            // 14773,28 ; tree
            // 14772,28 ; tree
            // 14771,28 ; tree
            // 14770,28 ; tree
            // 14780,27 ; tree
            // 14779,27 ; tree
            // 14778,27 ; tree
            // 14777,27 ; tree
            // 14776,27 ; tree
            // 14769,27 ; tree
            // 4339,26 ; pile of garbage
            // 7685,26 ; footprints
            // 14829,26 ; tree
            // 3170,26 ; turnip
            // 6082,25 ; snow
            // 6085,23 ; snow
            // 6947,22 ; leaves
            // 14854,22 ; tree
            // 14832,22 ; tree
            // 14838,22 ; tree
            // 6084,21 ; snow
            // 14839,21 ; tree
            // 3188,20 ; honeydew melon%s%
            // 14852,19 ; tree
            // 14851,18 ; tree
            // 6948,17 ; leaves
            // 14853,17 ; tree
            // 4833,15 ; curtain
            // 7684,15 ; footprints
            // 3330,15 ; cypress leaves
            // 14837,14 ; tree
            // 1,14 ; nodraw
            // 3194,14 ; canteloupe%s%
            // 6083,13 ; snow
            // 3264,13 ; orfluer flowers
            // 6950,13 ; leaves
            // 7686,13 ; footprints
            // 3165,13 ; watermelon%s%
            // 14840,12 ; tree
            // 3976,11 ; Nightshade
            // 4844,11 ; curtain
            // 14843,11 ; tree
            // 14841,11 ; tree
            // 14842,11 ; tree
            // 3276,10 ; tree
            // 2855,10 ; candelabra
            // 14827,10 ; tree
            // 4596,10 ; fur%s%
            // 3492,9 ; pear tree
            // 6292,9 ; ruined bed
            // 6294,9 ; ruined bed
            // 6295,9 ; ruined bed
            // 6293,9 ; ruined bed
            // 6949,9 ; leaves
            // 14828,9 ; tree
            // 15021,9 ; tree
            // 15978,9 ; sail
            // 15979,9 ; sail
            // 15982,9 ; spar
            // 15981,9 ; sail
            // 3982,8 ; Nox Crystal
            // 3476,8 ; apple tree
            // 3177,8 ; sprouts
            // 3187,8 ; squash
            // 3484,8 ; peach tree
            // 3981,7 ; Spider's Silk
            // 3980,7 ; Sulfurous Ash
            // 3480,7 ; apple tree
            // 15041,7 ; tree
            // 15040,7 ; tree
            // 15080,7 ; tree
            // 14833,7 ; tree
            // 14834,7 ; tree
            // 15011,7 ; tree
            // 3304,7 ; willow leaves
            // 3322,7 ; cypress leaves
            // 3973,6 ; Ginseng
            // 3481,6 ; leaves
            // 14836,6 ; tree
            // 14835,6 ; tree
            // 14867,6 ; tree
            // 15023,6 ; tree
            // 15020,6 ; tree
            // 15019,6 ; tree
            // 14844,6 ; tree
            // 14845,6 ; tree
            // 14846,6 ; tree
            // 15039,6 ; tree
            // 15010,6 ; tree
            // 15009,6 ; tree
            // 15008,6 ; tree
            // 15007,6 ; tree
            // 14868,6 ; tree
            // 3282,6 ; leaves
            // 3971,5 ; Executioner's Cap%
            // 3972,5 ; Garlic
            // 3985,5 ; Wyrm's Heart%s%
            // 3960,5 ; Batwing%s%
            // 3488,5 ; peach tree
            // 3496,5 ; pear tree
            // 4842,5 ; curtain
            // 15042,5 ; tree
            // 15044,5 ; tree
            // 14865,5 ; tree
            // 15043,5 ; tree
            // 15025,5 ; tree
            // 15022,5 ; tree
            // 15013,5 ; tree
            // 4831,5 ; curtain
            // 3267,5 ; muck
            // 3962,5 ; Black Pearl%s%
            // 3974,5 ; Mandrake Root%s%
            // 3486,5 ; peach tree
            // 3331,5 ; cypress leaves
            // 3327,5 ; cypress leaves
            // 3821,4 ; gold coin
            // 15045,4 ; tree
            // 12488,4 ; leaves
            // 15028,4 ; tree
            // 15027,4 ; tree
            // 15026,4 ; tree
            // 15012,4 ; tree
            // 15074,4 ; tree
            // 15075,4 ; tree
            // 15076,4 ; tree
            // 14831,4 ; tree
            // 14830,4 ; tree
            // 15071,4 ; tree
            // 15081,4 ; tree
            // 15082,4 ; tree
            // 15083,4 ; tree
            // 14864,4 ; tree
            // 14866,4 ; tree
            // 3173,4 ; gourd%s%
            // 15024,4 ; tree
            // 3485,4 ; leaves
            // 3285,4 ; leaves
            // 3514,4 ; seaweed
            // 3515,4 ; seaweed
            // 3174,4 ; gourd%s%
            // 3325,4 ; cypress leaves
            // 3328,4 ; cypress leaves
            // 3979,3 ; Pumice
            // 3478,3 ; apple tree
            // 3477,3 ; leaves
            // 14906,3 ; tree
            // 14905,3 ; tree
            // 14907,3 ; tree
            // 14908,3 ; tree
            // 14909,3 ; tree
            // 14910,3 ; tree
            // 14911,3 ; tree
            // 14916,3 ; tree
            // 14915,3 ; tree
            // 14914,3 ; tree
            // 14913,3 ; tree
            // 14899,3 ; tree
            // 14900,3 ; tree
            // 14901,3 ; tree
            // 14903,3 ; tree
            // 14902,3 ; tree
            // 14904,3 ; tree
            // 14912,3 ; tree
            // 15078,3 ; tree
            // 15077,3 ; tree
            // 15085,3 ; tree
            // 15048,3 ; tree
            // 14857,3 ; tree
            // 14858,3 ; tree
            // 14859,3 ; tree
            // 14860,3 ; tree
            // 14861,3 ; tree
            // 14862,3 ; tree
            // 14863,3 ; tree
            // 15052,3 ; tree
            // 15061,3 ; tree
            // 15059,3 ; tree
            // 15058,3 ; tree
            // 15057,3 ; tree
            // 15060,3 ; tree
            // 3181,3 ; onion%s%
            // 14803,3 ; tree
            // 14802,3 ; tree
            // 14801,3 ; tree
            // 14799,3 ; tree
            // 14798,3 ; tree
            // 14800,3 ; tree
            // 14797,3 ; tree
            // 14796,3 ; tree
            // 14792,3 ; tree
            // 14787,3 ; tree
            // 14790,3 ; tree
            // 14788,3 ; tree
            // 14789,3 ; tree
            // 14791,3 ; tree
            // 3983,3 ; Grave Dust
            // 3191,3 ; carrot%s%
            // 3279,3 ; leaves
            // 7683,3 ; footprints
            // 3175,3 ; gourd%s%
            // 3967,2 ; Brimstone
            // 3824,2 ; silver coin
            // 4839,2 ; curtain
            // 3977,2 ; Obsidian
            // 3493,2 ; leaves
            // 3497,2 ; leaves
            // 3489,2 ; leaves
            // 3482,2 ; apple tree
            // 12491,2 ; leaves
            // 12490,2 ; leaves
            // 15035,2 ; tree
            // 15038,2 ; tree
            // 15029,2 ; tree
            // 15073,2 ; tree
            // 15072,2 ; tree
            // 15031,2 ; tree
            // 15036,2 ; tree
            // 15086,2 ; tree
            // 7950,2 ; morning glory
            // 7949,2 ; morning glory
            // 15014,2 ; tree
            // 15015,2 ; tree
            // 14876,2 ; tree
            // 14874,2 ; tree
            // 14875,2 ; tree
            // 14877,2 ; tree
            // 14872,2 ; tree
            // 14873,2 ; tree
            // 14869,2 ; tree
            // 14870,2 ; tree
            // 14871,2 ; tree
            // 15049,2 ; tree
            // 14897,2 ; tree
            // 15046,2 ; tree
            // 15047,2 ; tree
            // 15079,2 ; tree
            // 3182,2 ; onion%s%
            // 15054,2 ; tree
            // 15055,2 ; tree
            // 15056,2 ; tree
            // 14793,2 ; tree
            // 3975,2 ; Eye%s% of Newt
            // 3185,2 ; head%s% of lettuce
            // 3961,2 ; Blackmoor
            // 3494,2 ; pear tree
            // 3301,2 ; walnut leaves
            // 3978,2 ; Pig Iron
            // 3292,2 ; oak leaves
            // 3172,2 ; gourd%s%
            // 8057,2 ; fur
            // 3275,1 ; tree
            // 9966,1 ; plum tree
            // 9965,1 ; plum tree
            // 8055,1 ; fur
            // 8056,1 ; fur
            // 4843,1 ; curtain
            // 6868,1 ; nest with eggs
            // 12493,1 ; leaves
            // 12489,1 ; leaves
            // 14761,1 ; tree
            // 14764,1 ; tree
            // 14763,1 ; tree
            // 14762,1 ; tree
            // 15037,1 ; tree
            // 12498,1 ; leaves
            // 12495,1 ; leaves
            // 15050,1 ; tree
            // 12494,1 ; leaves
            // 12492,1 ; leaves
            // 12499,1 ; leaves
            // 15033,1 ; tree
            // 15034,1 ; tree
            // 15032,1 ; tree
            // 15030,1 ; tree
            // 14898,1 ; tree
            // 14810,1 ; tree
            // 15084,1 ; tree
            // 15005,1 ; tree
            // 15006,1 ; tree
            // 3176,1 ; sprouts
            // 15053,1 ; tree
            // 14795,1 ; tree
            // 14794,1 ; tree
            // 15017,1 ; tree
            // 15016,1 ; tree
            // 15018,1 ; tree
            // 6296,1 ; ruined bed
            // 6298,1 ; ruined bed
            // 6299,1 ; ruined bed
            // 6297,1 ; ruined bed
            // 3171,1 ; turnip
            // 3192,1 ; carrot%s%
            // 3969,1 ; Fertile Dirt
            // 3487,1 ; fall leaves
            // 3490,1 ; peach tree
            // 3498,1 ; pear tree
            // 3479,1 ; fall leaves
            // 3298,1 ; walnut leaves
            return table;
        }
    }
}
