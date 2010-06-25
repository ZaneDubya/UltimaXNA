using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.UILegacy.Gumplings;

namespace UltimaXNA.UILegacy.ClientsideGumps
{
    class SkillsGump : Gump
    {
        public SkillsGump()
            : base(0, 0)
        {
            AddGumpling(new ExpandableScroll(this, 0, 0, 0, 200));
            ((ExpandableScroll)LastGumpling).TitleGumpID = 0x834;
        }
    }
}
