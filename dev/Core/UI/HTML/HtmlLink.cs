using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UltimaXNA.Core.UI.HTML.Styles;

namespace UltimaXNA.Core.UI.HTML
{
    public class HtmlLink
    {
        public Rectangle Area;
        public int Index;
        public string HREF;
        public StyleState Style;

        public HtmlLink(int i, string href, StyleState style)
        {
            Area = new Rectangle();
            Index = i;
            HREF = href;
            Style = style;
        }
    }
}
