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
#endregion

namespace UltimaXNA.Configuration
{
    public class GumpSettings : ASettingsSection
    {
        public const string SectionName = "gump";

        public Dictionary<string, Point> LastPositions
        {
            get;
            set;
        }

        public List<SavedGumpConfig> SavedGumps
        {
            get;
            set;
        }

        public GumpSettings()
        {
            LastPositions = new Dictionary<string, Point>();
            SavedGumps = new List<SavedGumpConfig>();
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
