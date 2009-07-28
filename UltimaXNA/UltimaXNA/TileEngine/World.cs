/***************************************************************************
 *   World.cs
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
    public static class World
    {
        private static Map _map;
        public static Map Map
        {
            get { return _map; }
        }

        public static int CenterX { get; set; }
        public static int CenterY { get; set; }
        public static int RenderBeginX { get; set; }
        public static int RenderBeginY { get; set; }
        public static int MaxRoofAltitude { get; internal set; }

        static World()
        {
            _map = new Map(0, 40, 0, 0);
        }

        public static void Update(GameTime gameTime)
        {
            if (GameState.InWorld)
            {
                Movement iCenterPosition = EntitiesCollection.GetPlayerObject().Movement;

                if ((CenterX != iCenterPosition.DrawPosition.TileX) ||
                    (CenterY != iCenterPosition.DrawPosition.TileY))
                {
                    CenterX = iCenterPosition.DrawPosition.TileX;
                    CenterY = iCenterPosition.DrawPosition.TileY;
                    RenderBeginX = iCenterPosition.DrawPosition.TileX - Map.GameSize / 2;
                    RenderBeginY = iCenterPosition.DrawPosition.TileY - Map.GameSize / 2;
                    _map.Update(CenterX, CenterY);
                    // Are we inside (under a roof)? Do not draw tiles above our head.
                    if (_map.GetMapCell(CenterX, CenterY).UnderRoof(iCenterPosition.DrawPosition.TileZ))
                    {
                        MaxRoofAltitude = iCenterPosition.DrawPosition.TileZ + 20;
                    }
                    else
                    {
                        MaxRoofAltitude = 255;
                    }
                }
            }
        }
    }
}