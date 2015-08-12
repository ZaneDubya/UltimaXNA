/***************************************************************************
 *   Regions.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace UltimaXNA.Core.UI.HTML
{
    public class RegionList
    {
        List<Region> m_regions = new List<Region>();

        public Region this[int index]
        {
            get
            {
                if (m_regions.Count == 0)
                    return null;
                if (index >= m_regions.Count)
                    index = m_regions.Count - 1;
                if (index < 0)
                    index = 0;
                return m_regions[index];
            }
        }

        public int Count
        {
            get { return m_regions.Count; }
        }

        public Region AddRegion(HREFAttributes href)
        {
            m_regions.Add(new Region(m_regions.Count, href));
            return m_regions[m_regions.Count - 1];
        }

        public void Clear()
        {
            m_regions.Clear();
        }

        public Region RegionfromPoint(Point p)
        {
            int index = -1;
            for (int i = 0; i < m_regions.Count; i++)
            {
                if (m_regions[i].Area.Contains(p))
                    index = i;
            }
            if (index == -1)
                return null;
            else
                return m_regions[index];
        }

        public Region Region(int index)
        {
            return m_regions[index];
        }
    }
}
