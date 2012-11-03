using System.Collections.Generic;
/***************************************************************************
 *   EngineVars.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using Microsoft.Xna.Framework;
using UltimaXNA.Entity;
using UltimaXNA.Interface.Input;

namespace UltimaXNA.ClientVars
{
    public class EngineVars
    {
        public static float TheTime
        {
            get { return (float)TheGameTime.TotalGameTime.TotalSeconds; }
        }

        public static Direction CursorDirection { get; internal set; }

        // InWorld allows us to tell when our character object has been loaded in the world.
        public static bool InWorld { get; set; }

        public static bool EngineRunning { get; set; } // false = engine immediately quits.
        public static bool IsMinimized { get; set; }

        public const float SecondsBetweenClickAndPickUp = 0.8f; // this is close to what the legacy client uses.
        public const float SecondsForDoubleClick = 0.5f;

        private static Serial _lastTarget;
        public static Serial LastTarget
        {
            get { return _lastTarget; }
            set
            {
                _lastTarget = value;
                UltimaClient.Send(new Network.Packets.Client.GetPlayerStatusPacket(0x04, _lastTarget));
            }
        }
        public static bool WarMode
        {
            get { return (Entities.GetPlayerObject() != null) ? ((Mobile)Entities.GetPlayerObject()).IsWarMode : false; }
            set { ((Mobile)Entities.GetPlayerObject()).IsWarMode = value; }
        }

        // Maintain an accurate count of frames per second.
        static float _FPS = 0, _Frames = 0, _ElapsedSeconds = 0;
        public static int FPS { get { return (int)_FPS; } }
        internal static bool UpdateFPS(GameTime gameTime)
        {
            _Frames++;
            _ElapsedSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_ElapsedSeconds >= .5f)
            {
                _FPS = _Frames / _ElapsedSeconds;
                _ElapsedSeconds = 0;
                _Frames = 0;
                return true;
            }
            return false;
        }

        static int _desiredFPS = 60;
        public static int DesiredFPS
        {
            get { return _desiredFPS; }
            set { _desiredFPS = value; }
        }
        public static bool LimitFPS = true;

        public static MouseButton MouseButton_Interact = MouseButton.Left;
        public static MouseButton MouseButton_Move = MouseButton.Right;
        public static bool MouseEnabled = true;

        static int _map = -1;
        public static int Map { get { return _map; } set { _map = value; } }

        static int _mapCount = -1;
        public static int MapCount { get { return _mapCount; } set { _mapCount = value; } }

        static int _season = 0;
        public static int Season { get { return _season; } set { _season = value; } }

        static bool _minimapLarge = false;
        public static bool MiniMap_LargeFormat { get { return _minimapLarge; } set { _minimapLarge = value; } }

        static int _mapSizeInMemory = 16; // We maintain 16 cells (128 tiles) in memory.
        public static int MapCellsInMemory { get { return _mapSizeInMemory; } }
        static int _updateRangeInTiles = 32; // Any mobile / item beyond this range is removed from the client. RunUO's range is 24.
        public static int UpdateRange { get { return _updateRangeInTiles; } }

        static int _renderSize = 40;
        public static int RenderSize
        {
            get { return _renderSize; }
            set { _renderSize = value; }
        }

        static Point2D m_ScreenSize;
        public static Point2D ScreenSize
        {
            get { return m_ScreenSize; }
            set { m_ScreenSize = value; }
        }

        static GameTime _theGameTime;
        internal static GameTime TheGameTime
        {
            get { return _theGameTime; }
        }

        public static void Update(GameTime gameTime)
        {
            _theGameTime = gameTime;

            // input for debug variables.
            List<InputEventKB> keyEvents = InputState.GetKeyboardEvents();
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
                        Diagnostics.Dynamic.InvokeDebug();
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
        }
    }
}
