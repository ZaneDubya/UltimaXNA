﻿/***************************************************************************
 *   Shared.cs
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
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entities;
using UltimaXNA.Input;
#endregion

namespace UltimaXNA.TileEngine
{
    [Flags]
    public enum PickTypes : int
    {
        PickNothing = 0,
        PickObjects = 1,
        PickStatics = 2,
        PickGroundTiles = 4,
        PickEverything = PickObjects | PickStatics | PickGroundTiles
    }
}
