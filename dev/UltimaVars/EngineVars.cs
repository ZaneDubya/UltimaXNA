
/***************************************************************************
 *   EngineVars.cs
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using InterXLib.Input.Windows;
using Microsoft.Xna.Framework;
using UltimaXNA.UltimaEntities;
using UltimaXNA.UltimaPackets;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaWorld;
using System.Collections.Generic;
#endregion

namespace UltimaXNA.UltimaVars
{
    public class EngineVars
    {
        public static byte[] Version = new byte[4] { 6, 0, 6, 2 };

        public static Direction CursorDirection { get; internal set; }

        // InWorld allows us to tell when our character object has been loaded in the world.
        public static bool InWorld { get; set; }

        public static bool EngineRunning { get; set; } // false = engine immediately quits.

        public const float ClickAndPickUpMS = 800f; // this is close to what the legacy client uses.
        public const float DoubleClickMS = 400f;

        public static Serial PlayerSerial
        {
            get;
            set;
        }

        public static bool WarMode
        {
            get
            {
                return (EntityManager.GetPlayerObject() != null) ? 
                    ((Mobile)EntityManager.GetPlayerObject()).Flags.IsWarMode : 
                    false;
            }
        }

        /// <summary>
        ///  When AllLabels is true, all entites should display their name above their object.
        /// </summary>
        public static bool AllLabels
        {
            get;
            set;
        }

        // Maintain an accurate count of frames per second.
        static List<float> m_FPS = new List<float>();
        internal static int UpdateFPS(double frameMS)
        {
            while (m_FPS.Count > 9)
                m_FPS.RemoveAt(0);
            m_FPS.Add(1000.0f / (float)frameMS);

            float count = 0.0f;
            for (int i = 0; i < m_FPS.Count; i++)
            {
                count += m_FPS[i];
            }

            count /= m_FPS.Count;

            return (int)System.Math.Ceiling(count);
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

        static int m_mapCount = -1;
        public static int MapCount { get { return m_mapCount; } set { m_mapCount = value; } }

        static int m_season = 0;
        public static int Season { get { return m_season; } set { m_season = value; } }

        static bool m_minimapLarge = false;
        public static bool MiniMap_LargeFormat { get { return m_minimapLarge; } set { m_minimapLarge = value; } }

        static Point m_ScreenSize;
        public static Point ScreenSize
        {
            get { return m_ScreenSize; }
            set { m_ScreenSize = value; }
        }

        public static bool NewDiagonalMovement = false;
    }
}
