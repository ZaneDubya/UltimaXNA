using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.UILegacy
{
    public interface IUIManager
    {
        bool IsMouseOverUI { get; }
        Cursor Cursor { get; }
        Gump AddGump(Serial serial, Serial gumpID, string[] gumplings, string[] lines, int x, int y);
    }
}
