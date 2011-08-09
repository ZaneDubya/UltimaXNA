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

        // bool timesSet = false;
        // float timesStart;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            parseKeyboard();

            /*if (timesSet == false)
            {
                timesStart = ClientVars.TheTime;
                timesSet = true;
            }

            if (ClientVars.TheTime - timesStart < 20f)
            {
                float time = ClientVars.TheTime - timesStart;
                time %= 4f;
                if (time < 2f)
                {
                    _position.X--;
                    _position.Y++;
                }
                else
                {
                    _position.X++;
                    _position.Y--;
                }
            }*/

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
                foreach (MapObject o in World.Map.GetMapTile(x, y, true).Items)
                {
                    o.Z += editing_Value;
                }
                for (int iy = -2; iy <= 2; iy++)
                {
                    for (int ix = -2; ix <= 2; ix++)
                    {
                        World.Map.GetMapTile(ix + x, iy + y, true).GroundTile.FlushSurroundings();
                    }
                }
            }
        }

        int edit_Value = 1;

        private void edit_parseKeyboard()
        {
            for (int k = (int)WinKeys.D0; k <= (int)WinKeys.D9; k++)
            {
                if (Input.HandleKeyboardEvent(KeyboardEvent.Down, (WinKeys)k, false, true, false))
                    editing_Size = k - (int)WinKeys.D0;
                else if (Input.HandleKeyboardEvent(KeyboardEvent.Down, (WinKeys)k, false, true, false))
                    edit_Value = k - (int)WinKeys.D0;
            }

            if (ClientVars.DEBUG_HighlightMouseOverObjects)
            {
                if (Input.HandleMouseEvent(MouseEvent.Down, MouseButton.Left))
                    edit_Begin(edit_Value);
                else if (Input.HandleMouseEvent(MouseEvent.Down, MouseButton.Right))
                    edit_Begin(-edit_Value);
            }

            if (edit_IsEditing)
            {
                if (Input.HandleMouseEvent(MouseEvent.Up, MouseButton.Left))
                    edit_End();
                if (Input.HandleMouseEvent(MouseEvent.Up, MouseButton.Right))
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

            if (Input.IsKeyDown(WinKeys.Up))
            {
                _position.X--;
                _position.Y--;
            }
            if (Input.IsKeyDown(WinKeys.Left))
            {
                _position.X--;
                _position.Y++;
            }
            if (Input.IsKeyDown(WinKeys.Down))
            {
                _position.X++;
                _position.Y++;
            }
            if (Input.IsKeyDown(WinKeys.Right))
            {
                _position.X++;
                _position.Y--;
            }
            if (World.Map != null)
            {
                int low = 0, high = 0;
                _position.Z = World.Map.GetAverageZ(_position.X, _position.Y, ref low, ref high);
            }

            if (Input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.W, false, true, false))
            {
                ClientVars.DEBUG_HighlightMouseOverObjects = Utility.ToggleBoolean(ClientVars.DEBUG_HighlightMouseOverObjects);
            }
            if (Input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.E, false, true, false))
            {
                ClientVars.DEBUG_DrawWireframe = Utility.ToggleBoolean(ClientVars.DEBUG_DrawWireframe);
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
