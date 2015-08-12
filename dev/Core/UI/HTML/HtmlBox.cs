using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UltimaXNA.Core.UI.HTML.Atoms;

namespace UltimaXNA.Core.UI.HTML
{
    class HtmlBox
    {
        public Rectangle Area;
        public List<AAtom> Contents = new List<AAtom>();

        public void AddAtom(AAtom atom)
        {
            Contents.Add(atom);
        }
    }
}
