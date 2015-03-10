using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Entity;

namespace UltimaXNA.UltimaGUI.Controls
{
    class GumpPicBackpack : GumpPic
    {
        public Item BackpackItem
        {
            get;
            protected set;
        }

        public GumpPicBackpack(Control owner, int page, int x, int y, Item backpack)
            : base(owner, page, x, y, 0xC4F6, 0)
        {
            BackpackItem = backpack;
        }
    }
}
