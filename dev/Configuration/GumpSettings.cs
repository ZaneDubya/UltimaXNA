/***************************************************************************
 *   GumpSettings.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Configuration;
using UltimaXNA.Configuration.Properties;
#endregion

namespace UltimaXNA.Configuration
{
    public class GumpSettings : ASettingsSection
    {
        /// <summary>
        /// The list of last positions where a given gump type was located.
        /// </summary>
        public Dictionary<string, Point> LastPositions
        {
            get;
            set;
        }

        /// <summary>
        /// A list of saved gumps, and data describing the same. These are reloaded when the world is started.
        /// </summary>
        public List<SavedGumpProperty> SavedGumps
        {
            get;
            set;
        }

        public GumpSettings()
        {
            LastPositions = new Dictionary<string, Point>();
            SavedGumps = new List<SavedGumpProperty>();
        }

        public Point GetLastPosition(string gumpID, Point defaultPosition)
        {
            Point value;
            if (LastPositions.TryGetValue(gumpID, out value))
                return value;
            else
                return defaultPosition;
        }

        public void SetLastPosition(string gumpID, Point position)
        {
            if (LastPositions.ContainsKey(gumpID))
                LastPositions[gumpID] = position;
            else
                LastPositions.Add(gumpID, position);
        }
    }
}
