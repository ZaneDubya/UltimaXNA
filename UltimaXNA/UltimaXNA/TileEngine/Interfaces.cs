/***************************************************************************
 *   Interfaces.cs
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
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.TileEngine
{
    public interface IWorld
    {
        Map Map { get; set; }
        int MaxRoofAltitude { get; }
    }

    public interface ITileEngine
    {
        void SetLightDirection(Vector3 nDirection);
        MapObject MouseOverObject { get; }
        MapObject MouseOverGroundTile { get; }
        PickTypes PickType { set; }
        int ObjectsRendered { get; }
        MiniMap MiniMap { get; }
		int OverallLightning { get; set; }
		int PersonalLightning { get; set; }
    }
}
