using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Configuration;

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

        public GumpSettings()
        {
            LastPositions = new Dictionary<string, Point>();
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
