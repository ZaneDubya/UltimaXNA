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

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using UltimaXNA.Core.UI.HTML.Styles;

namespace UltimaXNA.Core.UI.HTML
{
    public class HtmlLinkList
    {
        List<HtmlLink> m_regions = new List<HtmlLink>();

        public HtmlLink this[int index]
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

        public HtmlLink AddLink(string href, StyleState style)
        {
            m_regions.Add(new HtmlLink(m_regions.Count, href, style));
            return m_regions[m_regions.Count - 1];
        }

        public void Clear()
        {
            m_regions.Clear();
        }

        public HtmlLink RegionfromPoint(Point p)
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

        public HtmlLink Region(int index)
        {
            return m_regions[index];
        }
    }
}
