using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.UILegacy.Clientside
{
    class SkillsGump : Gump
    {
        public SkillsGump()
            : base(0, 0)
        {
            AddGumpling(new Gumplings.ExpandableScroll(this, 0, 0, 0, 200));
        }
    }
}
