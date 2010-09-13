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
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.Entities;
using UltimaXNA.Input;
using UltimaXNA.TileEngine;
#endregion

namespace UltimaXNA.SceneManagement
{
    public class DebugScene : BaseScene
    {
        Position3D _position = new Position3D(4312, 966, 0); // new Position3D(1453, 1561, 0);

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
            UI.AddGump_Local(new UILegacy.ClientsideGumps.DebugGump(), 40, 40);
            World.PickType = PickTypes.PickEverything;
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




        bool edit_IsEditing = false;
        int editing_Value = 0;
        int editing_Size = 0;
        List<Position3D> edit_Tiles = new List<Position3D>();

        private void edit_Begin(int editValue)
        {
            edit_Tiles.Clear();
            edit_IsEditing = true;
            editing_Value = editValue;
        }

        private void edit_End()
        {
            edit_IsEditing = false;
        }

        private void edit_AddPoint(int x, int y)
        {
            bool isTileEdited = false;
            foreach (Position3D p in edit_Tiles)
            {
                if ((p.X == x) && (p.Y == y))
                {
                    isTileEdited = true;
                    break;
                }
            }

            if (!isTileEdited)
            {
                edit_Tiles.Add(new Position3D(x, y, World.Map.GetMapTile(x, y, true).GroundTile.Z));
                foreach (MapObject o in World.Map.GetMapTile(x, y, true).GetSortedObjects())
                {
                    o.Z += editing_Value;
                }
                for (int iy = -2; iy <= 2; iy++)
                {
                    for (int ix = -2; ix <= 2; ix++)
                    {
                        World.Map.GetMapTile(ix + x, iy + y, true).GroundTile.MustUpdateSurroundings = true;
                    }
                }
            }
        }

        int edit_Value = 1;

        private void edit_parseKeyboard()
        {
            if (Input.IsKeyDown(Keys.LeftAlt))
            {
                for (int k = (int)Keys.D0; k <= (int)Keys.D9; k++)
                {
                    if (Input.IsKeyPress((Keys)k))
                    {
                        editing_Size = k - (int)Keys.D0;
                    }
                }
            }
            else
            {
                for (int k = (int)Keys.D0; k <= (int)Keys.D9; k++)
                {
                    if (Input.IsKeyPress((Keys)k))
                    {
                        edit_Value = k - (int)Keys.D0;
                    }
                }
            }

            if (ClientVars.DEBUG_HighlightMouseOverObjects)
            {
                if (Input.IsMouseButtonPress(MouseButtons.LeftButton))
                    edit_Begin(edit_Value);
                else if (Input.IsMouseButtonPress(MouseButtons.RightButton))
                    edit_Begin(-edit_Value);
            }

            if (edit_IsEditing)
            {
                if (Input.IsMouseButtonRelease(MouseButtons.LeftButton))
                    edit_End();
                if (Input.IsMouseButtonRelease(MouseButtons.RightButton))
                    edit_End();
            }

            if (edit_IsEditing && World.MouseOverObject != null)
            {
                int x = (int)World.MouseOverObject.Position.X;
                int y = (int)World.MouseOverObject.Position.Y;
                for (int iy = -editing_Size; iy <= editing_Size; iy++)
                    for (int ix = -editing_Size; ix <= editing_Size; ix++)
                    {
                        edit_AddPoint(x + ix, y + iy);
                    }
            }
                
        }

        private void parseKeyboard()
        {

            edit_parseKeyboard();

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
            if (World.Map != null)
            {
                int low = 0, high = 0;
                _position.Z = World.Map.GetAverageZ(_position.X, _position.Y, ref low, ref high);
            }
            // alt keys to change debug variables.
            if (Input.IsKeyDown(Keys.LeftAlt))
            {
                if (Input.IsKeyPress(Keys.W))
                {
                    ClientVars.DEBUG_HighlightMouseOverObjects = Utility.ToggleBoolean(ClientVars.DEBUG_HighlightMouseOverObjects);
                }
                if (Input.IsKeyPress(Keys.E))
                {
                    ClientVars.DEBUG_DrawWireframe = Utility.ToggleBoolean(ClientVars.DEBUG_DrawWireframe);
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
