/***************************************************************************
 *   WorldScene.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.UI;
using UltimaXNA.Client;
using UltimaXNA.Entities;
#endregion

namespace UltimaXNA.SceneManagement
{
    public class DebugScene : BaseScene
    {
        DrawPosition _position = new DrawPosition();

        public DebugScene(Game game)
            : base(game)
        {
        }

        public override void Intitialize()
        {
            base.Intitialize();
            World.OverallLightning = 0x09;
            World.PersonalLightning = 0x09;
            World.LightDirection = -0.6f;
            _position.TileX = 1453;
            _position.TileY = 1561;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            parseKeyboard();
            World.CenterPosition = _position;
            World.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            World.Draw(gameTime);
        }

        private void parseKeyboard()
        {
            if (Input.IsKeyDown(Keys.Up))
            {
                _position.TileX--;
                _position.TileY--;
            }
            if (Input.IsKeyDown(Keys.Left))
            {
                _position.TileX--;
                _position.TileY++;
            }
            if (Input.IsKeyDown(Keys.Down))
            {
                _position.TileX++;
                _position.TileY++;
            }
            if (Input.IsKeyDown(Keys.Right))
            {
                _position.TileX++;
                _position.TileY--;
            }
            // alt keys to change debug variables.
            if (Input.IsKeyDown(Keys.LeftAlt))
            {
                if (Input.IsKeyPress(Keys.W))
                {
                    World.DEBUG_DrawTileOver = Utility.ToggleBoolean(World.DEBUG_DrawTileOver);
                }
            }
        }
    }
}
