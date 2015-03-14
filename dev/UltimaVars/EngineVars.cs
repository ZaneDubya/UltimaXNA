using InterXLib.Input.Windows;
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
using UltimaXNA.UltimaPackets;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaWorld;

namespace UltimaXNA.UltimaVars
{
    public class EngineVars
    {
        public static float TheTime
        {
            get { return (float)GameTime.TotalGameTime.TotalSeconds; }
        }

        public static Direction CursorDirection { get; internal set; }

        // InWorld allows us to tell when our character object has been loaded in the world.
        public static bool InWorld { get; set; }

        public static bool EngineRunning { get; set; } // false = engine immediately quits.

        public const float SecondsBetweenClickAndPickUp = 0.8f; // this is close to what the legacy client uses.
        public const float SecondsForDoubleClick = 0.5f;

        private static Serial m_lastTarget;
        public static Serial LastTarget
        {
            get { return m_lastTarget; }
            set
            {
                m_lastTarget = value;
                UltimaInteraction.SendLastTargetPacket(m_lastTarget);
            }
        }
        public static bool WarMode
        {
            get { return (EntityManager.GetPlayerObject() != null) ? ((Mobile)EntityManager.GetPlayerObject()).IsWarMode : false; }
            set { ((Mobile)EntityManager.GetPlayerObject()).IsWarMode = value; }
        }

        // Maintain an accurate count of frames per second.
        static float m_FPS = 0, m_Frames = 0, m_ElapsedSeconds = 0;
        public static int FPS { get { return (int)m_FPS; } }
        internal static bool UpdateFPS(GameTime gameTime)
        {
            m_Frames++;
            m_ElapsedSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (m_ElapsedSeconds >= .5f)
            {
                m_FPS = m_Frames / m_ElapsedSeconds;
                m_ElapsedSeconds = 0;
                m_Frames = 0;
                return true;
            }
            return false;
        }

        static int m_desiredFPS = 60;
        public static int DesiredFPS
        {
            get { return m_desiredFPS; }
            set { m_desiredFPS = value; }
        }
        public static bool LimitFPS = true;

        public static MouseButton MouseButton_Interact = MouseButton.Left;
        public static MouseButton MouseButton_Move = MouseButton.Right;
        public static bool MouseEnabled = true;

        static int m_map = -1;
        public static int Map
        {
            get { return m_map; } 
            set
            {
                if (m_map != value)
                {
                    m_map = value;
                    UltimaWorld.View.IsometricRenderer.Map = new Map(m_map);
                }
            }
        }

        static int m_mapCount = -1;
        public static int MapCount { get { return m_mapCount; } set { m_mapCount = value; } }

        static int m_season = 0;
        public static int Season { get { return m_season; } set { m_season = value; } }

        static bool m_minimapLarge = false;
        public static bool MiniMap_LargeFormat { get { return m_minimapLarge; } set { m_minimapLarge = value; } }

        static int m_mapSizeInMemory = 16; // We maintain 16 cells (128 tiles) in memory.
        public static int MapCellsInMemory { get { return m_mapSizeInMemory; } }
        static int m_updateRangeInTiles = 32; // Any mobile / item beyond this range is removed from the client. RunUO's range is 24.
        public static int UpdateRange { get { return m_updateRangeInTiles; } }

        static int m_renderSize = 40;
        public static int RenderSize
        {
            get { return m_renderSize; }
            set { m_renderSize = value; }
        }

        static Point m_ScreenSize;
        public static Point ScreenSize
        {
            get { return m_ScreenSize; }
            set { m_ScreenSize = value; }
        }

        static GameTime m_theGameTime;
        internal static GameTime GameTime
        {
            get { return m_theGameTime; }
            set
            {
                m_theGameTime = value;
                UpdateFPS(m_theGameTime);
            }
        }
    }
}
