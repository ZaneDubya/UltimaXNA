using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.UILegacy.Gumplings;
using UltimaXNA.Data;

namespace UltimaXNA.UILegacy.ClientsideGumps
{
    class SkillsGump : Gump
    {
        ExpandableScroll _scroll;
        HtmlGump _list;
        public SkillsGump()
            : base(0, 0)
        {
            AddGumpling(_scroll = new ExpandableScroll(this, 0, 0, 0, 200));
            _scroll.TitleGumpID = 0x834;
            _scroll.MakeDragger(this);
            _scroll.MakeCloseTarget(this);
            IsMovable = true;

            AddGumpling(_list = new HtmlGump(this, 0, 10, 20, 180, 100, 0, 1, ""));
            _list.Text = buildSkillsString();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            _list.X = 26;
            _list.Y = 33;
            _list.Width = this.Width - 56;
            _list.Height = this.Height - 95;
            base.Update(gameTime);
        }

        public override void ActivateByHREF(string href)
        {
            if (href.Substring(0, 6) == "skill=")
            {
                int skillIndex;
                if (!int.TryParse(href.Substring(6), out skillIndex))
                        return;
                Interaction.UseSkill(skillIndex);
            }
            base.ActivateByHREF(href);
        }

        private string buildSkillsString()
        {
            StringBuilder str = new StringBuilder();

            Skill[] skills = Data.Skills.List;
            foreach (Skill skill in skills)
            {
                if (skill.UseButton)
                {
                    str.Append("<a href='skill=" + skill.Index +
                        "' color='5b4f29' hovercolor='857951' activecolor='402708' text-decoration=none>" +
                        "<gumpimg src='2103' hoversrc='2104' activesrc='2103'/><span width='2'/>" + skill.Name + "</a><br/>");
                }
                else
                {
                    str.Append("<span width='14'/><medium color=50422D>" + skill.Name + "</medium><br/>");
                }
            }
            return str.ToString();
        }
    }
}
