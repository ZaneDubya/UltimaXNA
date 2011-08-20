using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using UltimaXNA.Diagnostics;

namespace UltimaXNA.UILegacy.HTML
{
    public class HTMLRegions
    {
        List<HTMLRegion> _regions = new List<HTMLRegion>();

        public List<HTMLRegion> Regions
        {
            get
            {
                return _regions;
            }
        }

        public int Count
        {
            get { return _regions.Count; }
        }

        public HTMLRegion AddRegion(HREF_Attributes href)
        {
            _regions.Add(new HTMLRegion(_regions.Count, href));
            return _regions[_regions.Count - 1];
        }

        public void Clear()
        {
            _regions.Clear();
        }

        public HTMLRegion RegionfromPoint(Point p)
        {
            int index = -1;
            for (int i = 0; i < _regions.Count; i++)
            {
                if (_regions[i].Area.Contains(p))
                    index = i;
            }
            if (index == -1)
                return null;
            else
                return _regions[index];
        }

        public HTMLRegion Region(int index)
        {
            return _regions[index];
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
