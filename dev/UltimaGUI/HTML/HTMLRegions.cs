/***************************************************************************
 *   HTMLRegions.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace UltimaXNA.UltimaGUI.HTML
{
    public class HTMLRegions
    {
        List<HTMLRegion> m_regions = new List<HTMLRegion>();

        public List<HTMLRegion> Regions
        {
            get
            {
                return m_regions;
            }
        }

        public int Count
        {
            get { return m_regions.Count; }
        }

        public HTMLRegion AddRegion(HREF_Attributes href)
        {
            m_regions.Add(new HTMLRegion(m_regions.Count, href));
            return m_regions[m_regions.Count - 1];
        }

        public void Clear()
        {
            m_regions.Clear();
        }

        public HTMLRegion RegionfromPoint(Point p)
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

        public HTMLRegion Region(int index)
        {
            return m_regions[index];
        }
    }

    public class HTMLRegion
    {
        public Rectangle Area;
        public int Index;
        public HREF_Attributes HREFAttributes;

        public HTMLRegion(int i, HREF_Attributes data)
        {
            Area = new Rectangle();
            HREFAttributes = data;
            Index = i;
        }
    }

    public class HREF_Attributes
    {
        public string HREF;
        public int UpHue = 1;
        public int OverHue = 31;
        public int DownHue = 11;
        public bool Underline = true;
    }
}
