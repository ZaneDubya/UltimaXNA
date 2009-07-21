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

    public interface IMapObject
    {
        int ID { get; }
        int SortZ { get; }
        int Threshold { get; }
        int Tiebreaker { get; }
        MapObjectTypes Type { get; }
        Vector2 Position { get; }
        int Z { get; }
        int OwnerSerial { get; }
        Entities.Entity OwnerEntity { get; }
    }

    public interface ITileEngine
    {
        void SetLightDirection(Vector3 nDirection);
        IMapObject MouseOverObject { get; }
        IMapObject MouseOverGroundTile { get; }
        PickTypes PickType { set; }
        int ObjectsRendered { get; }
        MiniMap MiniMap { get; }
    }
}
