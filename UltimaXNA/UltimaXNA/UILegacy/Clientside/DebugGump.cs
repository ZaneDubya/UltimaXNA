using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.UILegacy.Gumplings;

namespace UltimaXNA.UILegacy.Clientside
{
    public class DebugGump : Gump 
    {
        public DebugGump(Serial serial)
            : base(serial, 0)
        {
            // minimized view
            this.AddGumpling(new Gumplings.ResizePic(this, 1, 0, 0, 0x2486, 16, 16));
            this.AddGumpling(new Gumplings.Button(this, 1, 0, 0, 0x37, 0x38, 0, 2, 0));
            // maximized view
            this.AddGumpling(new Gumplings.ResizePic(this, 2, 0, 0, 0x2486, 256, 256));
            this.AddGumpling(new Gumplings.Button(this, 2, 0, 0, 0x38, 0x37, 0, 1, 0));
            this.AddGumpling(new Gumplings.TextLabel(this, 2, 22, 0, 0, "Debug Gump"));
            this.AddGumpling(new Gumplings.TextEntry(this, 2, 8, 22, 200, 20, 0, 0, 0, "Debug Gump"));
        }
    }
}
