/***************************************************************************
 *   PickType.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using System;
using UltimaXNA.Ultima.World.Entities;
#endregion

namespace UltimaXNA.Ultima.World.Input
{
    [Flags]
    public enum PickType : int
    {
        PickNothing = 0,
        PickObjects = 1,
        PickStatics = 2,
        PickGroundTiles = 4,
        PickEverything = PickObjects | PickStatics | PickGroundTiles
    }
}