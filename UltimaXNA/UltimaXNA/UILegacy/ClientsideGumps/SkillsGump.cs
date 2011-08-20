using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.UILegacy.Gumplings;

namespace UltimaXNA.UILegacy.ClientsideGumps
{
    class SkillsGump : Gump
    {
        ListGumpling _list;
        public SkillsGump()
            : base(0, 0)
        {
            AddGumpling(new ExpandableScroll(this, 0, 0, 0, 200));
            ((ExpandableScroll)LastGumpling).TitleGumpID = 0x834;
            AddGumpling(_list = new ListGumpling(this, 0, 10, 20, 180, 100));
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            _list.X = 26;
            _list.Y = 33;
            _list.Width = this.Width - 56;
            _list.Height = this.Height - 98;
            base.Update(gameTime);
        }
    }
}
