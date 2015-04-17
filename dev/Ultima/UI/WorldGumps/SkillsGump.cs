/***************************************************************************
 *   SkillsGump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using System.Text;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World;

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class SkillsGump : Gump
    {
        private ExpandableScroll m_scroll;
        private HtmlGump m_list;

        private WorldModel m_World;

        public SkillsGump()
            : base(0, 0)
        {
            m_World = UltimaServices.GetService<WorldModel>();

            AddControl(m_scroll = new ExpandableScroll(this, 0, 0, 0, 200));
            m_scroll.TitleGumpID = 0x834;

            AddControl(m_list = new HtmlGump(this, 0, 10, 20, 180, 100, 0, 1, ""));
        }

        public override void Update(double totalMS, double frameMS)
        {
            m_list.X = 26;
            m_list.Y = 33;
            m_list.Width = Width - 56;
            m_list.Height = Height - 95;
            //if (m_lastUpdateCount != Ultima.ClientVars.Skills.UpdateCount)  Could not resolve this...
                m_list.Text = buildSkillsString();
            base.Update(totalMS, frameMS);
        }

        public override void ActivateByHREF(string href)
        {
            if (href.Substring(0, 6) == "skill=")
            {
                int skillIndex;
                if (!int.TryParse(href.Substring(6), out skillIndex))
                        return;
                m_World.Interaction.UseSkill(skillIndex);
            }
            m_list.Text = buildSkillsString();
            base.ActivateByHREF(href);
        }

        private string buildSkillsString()
        {
            // m_lastUpdateCount = Ultima.ClientVars.Skills.UpdateCount; Could not resolve this...
            StringBuilder str = new StringBuilder();

            foreach (Player.SkillEntry skill in PlayerState.Skills.List.Values)
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
