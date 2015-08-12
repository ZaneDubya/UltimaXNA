using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Core.UI.HTML.Layout
{
    class LayoutTree
    {
        // ======================================================================
        // Public properties
        // ======================================================================

        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }

        public List<Box> Boxes
        {
            get;
            private set;
        }

        // ======================================================================
        // Ctor
        // ======================================================================

        public LayoutTree(string html, int maxWidth)
        {

        }
    }
}
