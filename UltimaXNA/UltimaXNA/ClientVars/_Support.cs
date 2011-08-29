/***************************************************************************
 *   _Support.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Extensions;
using UltimaXNA.Input;
using UltimaXNA.TileEngine;
using UltimaXNA.UILegacy;

namespace UltimaXNA.ClientVars
{
    class _Support : GameComponent
    {
        static Game _game;
        static IInputState _Input;
        static IUIManager _UserInterface;
        static IIsometricRenderer _World;

        internal static IIsometricRenderer World
        {
            get { return _World; }
        }
        internal static GraphicsDevice Graphics
        {
            get { return _game.GraphicsDevice; }
        }

        static GameTime _theTime;
        internal static GameTime TheTime
        {
            get { return _theTime; }
        }

        public _Support(Game game)
            : base(game)
        {
            _game = game;
            _Input = Game.Services.GetService<IInputState>();
            _UserInterface = Game.Services.GetService<IUIManager>();
            _World = game.Services.GetService<IIsometricRenderer>();
            EngineVars.EngineRunning = true;
            EngineVars.InWorld = false;
        }

        public override void Update(GameTime gameTime)
        {
            _theTime = gameTime;

            // input for debug variables.
            List<InputEventKB> keyEvents = _Input.GetKeyboardEvents();
            foreach (InputEventKB e in keyEvents)
            {
                // debug flags
                if ((e.EventType == KeyboardEvent.Press) && (e.KeyCode == WinKeys.D) && e.Control)
                {
                    if (!e.Alt)
                    {

                        if (!DebugVars.Flag_ShowDataRead)
                            DebugVars.Flag_ShowDataRead = true;
                        else
                        {
                            if (!DebugVars.Flag_BreakdownDataRead)
                                DebugVars.Flag_BreakdownDataRead = true;
                            else
                            {
                                DebugVars.Flag_ShowDataRead = false;
                                DebugVars.Flag_BreakdownDataRead = false;
                            }
                        }
                    }
                    else
                    {
                        Diagnostics.Dynamic.InvokeDebug(Game);
                    }
                    e.Handled = true;
                }

                // fps limiting
                if ((e.EventType == KeyboardEvent.Press) && (e.KeyCode == WinKeys.F) && e.Alt)
                {
                    if (!e.Control)
                        DebugVars.Flag_DisplayFPS = Utility.ToggleBoolean(DebugVars.Flag_DisplayFPS);
                    else
                        EngineVars.LimitFPS = Utility.ToggleBoolean(EngineVars.LimitFPS);
                    e.Handled = true;
                }

                // mouse enabling
                if ((e.EventType == KeyboardEvent.Press) && (e.KeyCode == WinKeys.M) && e.Alt)
                {
                    EngineVars.MouseEnabled = Utility.ToggleBoolean(EngineVars.MouseEnabled);
                    e.Handled = true;
                }
            }

            ClientVars.EngineVars.UpdateFPS(gameTime);

            base.Update(gameTime);
        }
    }
}
