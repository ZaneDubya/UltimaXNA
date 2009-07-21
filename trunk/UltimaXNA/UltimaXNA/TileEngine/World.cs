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
    public class World : GameComponent, IWorld
    {
        #region Map
        private Map m_Map;
        public Map Map
        {
            get { return m_Map; }
            set { m_Map = value; }
        }
        #endregion

        private IEntitiesService m_GameObjectsService;
        private IGameState m_GameStateService;

        public World(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IWorld), this);
        }

        public override void Initialize()
        {
            base.Initialize();
            m_Map = new Map(0, 40, 0, 0);
            m_GameObjectsService = (IEntitiesService)Game.Services.GetService(typeof(IEntitiesService));
            m_GameStateService = (IGameState)Game.Services.GetService(typeof(IGameState));
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (m_GameStateService.InWorld)
            {
                Movement iCenterPosition = m_GameObjectsService.GetPlayerObject().Movement;

                if ((X != iCenterPosition.DrawPosition.TileX) ||
                    (Y != iCenterPosition.DrawPosition.TileY))
                {
                    X = iCenterPosition.DrawPosition.TileX;
                    Y = iCenterPosition.DrawPosition.TileY;
                    m_Map.Update(X, Y);
                    // Are we inside (under a roof)? Do not draw tiles above our head.
                    if (m_Map.GetMapCell(X, Y).UnderRoof(iCenterPosition.DrawPosition.TileZ))
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
        public static int X { get; set; }
        public static int Y { get; set; }
        public int MaxRoofAltitude { get; set; }
    }
}