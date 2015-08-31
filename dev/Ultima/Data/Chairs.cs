/***************************************************************************
 *   Chairs.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

#region usings
using System.Collections.Generic;
using UltimaXNA.Ultima.World;
#endregion

namespace UltimaXNA.Ultima.Data
{
    public static class Chairs
    {
        private static Dictionary<int, ChairData> m_Chairs;

        static Chairs()
        {
            m_Chairs = new Dictionary<int, ChairData>();
            //0x0B2C - 0x0B33
            //0x0B4E - 0x0B6A
            //0x1218 - 0x121B
            m_Chairs.Add(0x0B2C, new ChairData(0x0B2C, Direction.South, ChairType.SingleFacing));
        }

        public bool CheckItemAsChair(int itemID, out ChairData value)
        {
            if (m_Chairs.TryGetValue(itemID, out value))
            {
                return true;
            }
            else
            {
                value = ChairData.Null;
                return false;
            }
        }

        public struct ChairData
        {
            public readonly int ItemID;
            public readonly Direction Facing;
            public readonly ChairType ChairType;

            public static ChairData Null = new ChairData(0, Direction.ValueMask, ChairType.AnyFacing);

            /// <summary>
            /// Creates a new chair data object.
            /// </summary>
            /// <param name="itemID">ItemID of the chair.</param>
            /// <param name="facing">The valid facing of the chair. Must be North, West, South, or East.</param>
            /// <param name="chairType">Whether the chair is a single facing (chair) reversible facing (bench) or any facing (stool) object.</param>
            public ChairData(int itemID, Direction facing, ChairType chairType)
            {
                ItemID = itemID;
                Facing = facing;
                ChairType = chairType;
            }
        }

        public enum ChairType
        {
            /// <summary>
            /// The chair has only one valid facing. The mobile defaults to being drawn in the single default facing.
            /// </summary>
            SingleFacing = 0,
            /// <summary>
            /// The chair has two valid facings which are mirrored. 
            /// </summary>
            ReversibleFacing = 1,
            /// <summary>
            /// Mobiles can face any direction so long as it is NWS or E. The mobile defaults to being drawn in the default facing until it attempts to switch to another valid facing.
            /// </summary>
            AnyFacing = 2
        }
    }
}
