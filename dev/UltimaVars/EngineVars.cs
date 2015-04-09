
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
using UltimaXNA.Input.Windows;
using Microsoft.Xna.Framework;
using UltimaXNA.UltimaEntities;
using UltimaXNA.UltimaPackets;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaWorld;
using System.Collections.Generic;
#endregion

namespace UltimaXNA.UltimaVars
{
    //TODO this should merge to something like Client or Engine.  All of these settings pertain to states that should be controlled by specific systems.
    // InWorld is a "Client" state
    // Version is a "Client" value
    // EngineRunning should be "Engine.IsRunning"
    // ClickAndPickupMS should be controlled by the InputManager
    // DoubleClickMS should be controlled by the InputManager
    // PlayerSerial needs to move to EntityManager
    // AllLabels should be "ShowAllLabels" and needs to be either in UI or Client
    // FPS calculations should be done by Client/Engine
    // MiniMap_LargeFormat, should be controlled by the radar itself.
    // NewDiagonalMovement should be a setting.
    public class EngineVars
    {
        public static byte[] Version = new byte[4] { 6, 0, 6, 2 };
        
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
        
        static bool m_minimapLarge = false;
        public static bool MiniMap_LargeFormat { get { return m_minimapLarge; } set { m_minimapLarge = value; } }


        public static bool NewDiagonalMovement = false;
    }
}
