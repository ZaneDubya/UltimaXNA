using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Ultima.World;

namespace UltimaXNA.Ultima.Data
{
    public static class Chairs
    {
        private static Dictionary<int, ChairData> m_Chairs;

        static Chairs()
        {
            m_Chairs = new Dictionary<int, ChairData>();
        }
        //public bool IsItemIDAChair
        //0x0B2C - 0x0B33
        //0x0B4E - 0x0B6A
        //0x1218 - 0x121B

        public struct ChairData
        {
            public readonly int ItemID;
            public readonly Direction Facing;
            public readonly ChairType ChairType;

            public ChairData(int itemID, Direction facing, ChairType chairType)
            {
                ItemID = itemID;
                Facing = facing;
                ChairType = chairType;
            }
        }

        public enum ChairType
        {
            SingleFacing = 0,
            ReversibleFacing = 1,
            AnyFacing = 2
        }
    }
}
