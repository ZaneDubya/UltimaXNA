/***************************************************************************
 *   MobileMoveEvent.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

namespace UltimaXNA.Ultima.World.Entities.Mobiles
{
    class MobileMoveEvent
    {
        public bool CreatedByPlayerInput = false;
        public readonly int X, Y, Z, Facing, Fastwalk;

        public MobileMoveEvent(int x, int y, int z, int facing, int fastwalk)
        {
            X = x;
            Y = y;
            Z = z;
            Facing = facing;
            Fastwalk = fastwalk;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", X, Y, Z);
        }
    }
}
