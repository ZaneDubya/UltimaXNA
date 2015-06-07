/***************************************************************************
 *   EngineVars.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

namespace UltimaXNA.Ultima
{
    //TODO this should merge to something like Client or Engine.  All of these settings pertain to states that should be controlled by specific systems.
    // InWorld is a "Client" state
    // EngineRunning should be "Engine.IsRunning"
    // AllLabels should be "ShowAllLabels" and needs to be either in UI or Client
    // MiniMap_LargeFormat, should be controlled by the radar itself.
    // NewDiagonalMovement should be a setting.
    public class EngineVars
    {
        // InWorld allows us to tell when our character object has been loaded in the world.
        public static bool InWorld { get; set; }
        public static bool EngineRunning { get; set; } // false = engine immediately quits.

        /// <summary>
        ///  When AllLabels is true, all entites should display their name above their object.
        /// </summary>
        public static bool AllLabels
        {
            get;
            set;
        }
        
        static bool m_minimapLarge = false;
        public static bool MiniMap_LargeFormat { get { return m_minimapLarge; } set { m_minimapLarge = value; } }

        public static bool NewDiagonalMovement = false;

        public static int MouseOverHue = 0x038;
    }
}
