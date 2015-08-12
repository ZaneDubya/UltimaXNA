using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace UltimaXNA.Core.UI.HTML
{
    public class Region
    {
        public Rectangle Area;
        public int Index;
        public HREFAttributes HREF;

        public Region(int i, HREFAttributes href)
        {
            Area = new Rectangle();
            HREF = href;
            Index = i;
        }
    }
}
