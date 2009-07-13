#region File Description & Usings
//-----------------------------------------------------------------------------
// World.cs
//
// Created by ClintXNA, modifications by Poplicola
//-----------------------------------------------------------------------------
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.TileEngine
{
    public interface IWorld
    {
        Map Map { get; set; }
        int MaxRoofAltitude { get; }
    }

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

        private GameObjects.IGameObjects m_GameObjectsService;
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
            m_GameObjectsService = (GameObjects.IGameObjects)Game.Services.GetService(typeof(GameObjects.IGameObjects));
            m_GameStateService = (IGameState)Game.Services.GetService(typeof(IGameState));
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (m_GameStateService.InWorld)
            {
                GameObjects.Movement iCenterPosition = m_GameObjectsService.GetPlayerObject().Movement;

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