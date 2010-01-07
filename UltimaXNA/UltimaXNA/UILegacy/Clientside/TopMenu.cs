using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.UILegacy.Gumplings;

namespace UltimaXNA.UILegacy.Clientside
{
    class TopMenu : Gump
    {
        public TopMenu(Serial serial)
            : base(serial, 0)
        {
            // maximized view
            AddGumpling(new ResizePic(this, 1, 0, 0, 9200, 610, 27));
            AddGumpling(new Button(this, 1, 5, 3, 5540, 5542, 0, 2, 0));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5541;
            // buttons are 2443 small, 2445 big
            // map, paperdollB, inventoryB, journalB, chat, help, < ? >
            // minimized view
            AddGumpling(new ResizePic(this, 2, 0, 0, 9200, 30, 27));
            AddGumpling(new Button(this, 2, 5, 3, 5537, 5539, 0, 1, 0));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5538;
        }
    }
}
