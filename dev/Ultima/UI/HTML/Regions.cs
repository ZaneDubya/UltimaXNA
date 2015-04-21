/***************************************************************************
 *   Regions.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace UltimaXNA.Ultima.UI.HTML
{
    public class Regions
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

        public Region AddRegion(HREF_Attributes href)
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

    public class Region
    {
        public Rectangle Area;
        public int Index;
        public HREF_Attributes HREFAttributes;

        public Region(int i, HREF_Attributes data)
        {
            Area = new Rectangle();
            HREFAttributes = data;
            Index = i;
        }
    }

    public class HREF_Attributes
    {
        public string HREF;
        public int UpHue = 4;
        public int OverHue = 6;
        public int DownHue = 2;
        public bool Underline = true;
    }
}
