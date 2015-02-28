/***************************************************************************
 *   SkillsGump.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using System.Text;
using UltimaXNA.UltimaGUI.Controls;

namespace UltimaXNA.UltimaGUI.Gumps
{
    class SkillsGump : Gump
    {
        private int _lastUpdateCount = Int32.MinValue;
        ExpandableScroll _scroll;
        HtmlGump _list;
        public SkillsGump()
            : base(0, 0)
        {
            AddControl(_scroll = new ExpandableScroll(this, 0, 0, 0, 200));
            _scroll.TitleGumpID = 0x834;
            _scroll.MakeDragger(this);
            _scroll.MakeCloseTarget(this);
            IsMovable = true;

            AddControl(_list = new HtmlGump(this, 0, 10, 20, 180, 100, 0, 1, ""));
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            _list.X = 26;
            _list.Y = 33;
            _list.Width = this.Width - 56;
            _list.Height = this.Height - 95;
            if (_lastUpdateCount != UltimaVars.Skills.UpdateCount)
                _list.Text = buildSkillsString();
            base.Update(gameTime);
        }

        public override void ActivateByHREF(string href)
        {
            if (href.Substring(0, 6) == "skill=")
            {
                int skillIndex;
                if (!int.TryParse(href.Substring(6), out skillIndex))
                        return;
                UltimaInteraction.UseSkill(skillIndex);
            }
            _list.Text = buildSkillsString();
            base.ActivateByHREF(href);
        }

        private string buildSkillsString()
        {
            _lastUpdateCount = UltimaVars.Skills.UpdateCount;
            StringBuilder str = new StringBuilder();

            foreach (UltimaVars.SkillEntry skill in UltimaVars.Skills.List.Values)
            {
                str.Append(string.Format(skill.HasUseButton ? kSkillName_UseButton : kSkillName_NoUseButton, skill.Index, skill.Name));
                str.Append(string.Format(kSkillValues[skill.LockType], skill.Value));
            }
            return str.ToString();
        }

        // 0 = skill index, 1 = skill name
        const string kSkillName_UseButton = "<left><a href='skill={0}' color='5b4f29' hovercolor='857951' activecolor='402708' text-decoration=none>" +
                        "<gumpimg src='2103' hoversrc='2104' activesrc='2103'/><span width='2'/>{1}</a></left>";
        const string kSkillName_NoUseButton = "<left><span width='14'/><medium color=50422D>{1}</medium></left>";
        // 0 = skill value
        static string[] kSkillValues = new string[3] {
            "<right>{0:0.0}<gumpimg src='2436' width='12' height='15'/></right><br/>",
            "<right>{0:0.0}<gumpimg src='2438' width='12' height='15'/></right><br/>",
            "<right>{0:0.0}<gumpimg src='2092' width='12' height='15'/></right><br/>" };
    }
}
