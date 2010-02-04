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
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.Entities;
#endregion

namespace UltimaXNA.SceneManagement
{
    public class DebugScene : BaseScene
    {
        Position3D _position = new Position3D(1453, 1561, 0);

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
            ClientVars.Map = 0;
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
            if (World.DEBUG_DrawTileOver && Input.IsMouseButtonPress(UltimaXNA.Input.MouseButtons.LeftButton))
            {
                int x = (int)World.MouseOverObject.Position.X;
                int y = (int)World.MouseOverObject.Position.Y;
                TileEngine.MapTile t = World.Map.GetMapTile(x, y);
                t.GroundTile.Z++;
                for (int iy = -1; iy < 2; iy++)
                    for (int ix = -1; ix < 2; ix++)
                    {
                        World.Map.UpdateSurroundings(World.Map.GetMapTile(x + ix, y + iy).GroundTile);
                    }
            }

            if (Input.IsKeyDown(Keys.Up))
            {
                _position.X--;
                _position.Y--;
            }
            if (Input.IsKeyDown(Keys.Left))
            {
                _position.X--;
                _position.Y++;
            }
            if (Input.IsKeyDown(Keys.Down))
            {
                _position.X++;
                _position.Y++;
            }
            if (Input.IsKeyDown(Keys.Right))
            {
                _position.X++;
                _position.Y--;
            }
            int z = 0, top = 0, avg = 0;
            if (World.Map != null)
            {
                World.Map.GetAverageZ(_position.X, _position.Y, ref z, ref avg, ref top);
                _position.Z = z;
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

        static void loadDebugAssembly(string filename)
        {
            if (System.IO.File.Exists(filename))
            {
                System.Reflection.Assembly debugAssembly = System.Reflection.Assembly.LoadFrom(filename);
                Object o = debugAssembly.CreateInstance("UltimaXNA.UXNADebug", true);
            }
        }
    }
}
