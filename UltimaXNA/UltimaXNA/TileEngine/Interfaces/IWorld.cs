/***************************************************************************
 *   IWorld.cs
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
using Microsoft.Xna.Framework;
using UltimaXNA.Entities;
#endregion

namespace UltimaXNA.TileEngine
{
    public interface IWorld
    {
        Map Map { get; }
        DrawPosition CenterPosition { get; set; }
        int RenderBeginX { get; set; }
        int RenderBeginY { get; set; }
        int MaxRoofAltitude { get; }
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);

        MapObject MouseOverObject { get; }
        MapObject MouseOverGroundTile { get; }
        PickTypes PickType { get; set; }
        int ObjectsRendered { get; }
        int PersonalLightning { get; set; }
        int OverallLightning { get; set; }
        float LightDirection { get; set; }

        bool DEBUG_DrawTileOver { get; set; }
        bool DEBUG_DrawDebug { get; set; }
    }
}
