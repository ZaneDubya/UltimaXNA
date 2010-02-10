using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.UILegacy.Gumplings;

namespace UltimaXNA.UILegacy.Clientside
{
    public class DebugGump : Gump 
    {
        public DebugGump()
            : base(0, 0)
        {
            int width = 200;
            // minimized view
            IsMovable = true;
            AddGumpling(new Gumplings.ResizePic(this, 1, 0, 0, 0x2486, width, 16));
            AddGumpling(new Gumplings.Button(this, 1, width - 18, 0, 2117, 2118, 0, 2, 0));
            // maximized view
            AddGumpling(new Gumplings.ResizePic(this, 2, 0, 0, 0x2486, width, 256));
            AddGumpling(new Gumplings.Button(this, 2, width - 18, 0, 2117, 2118, 0, 1, 0));
            AddGumpling(new Gumplings.TextLabel(this, 2, 2, 0, 0, "Debug Gump"));
        }
    }
}
