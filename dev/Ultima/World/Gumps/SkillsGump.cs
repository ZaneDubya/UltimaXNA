/***************************************************************************
 *   SkillsGump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Text;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.Controls;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.Ultima.World.Gumps
{
    class SkillsGump : Gump
    {
        private ExpandableScroll m_Background;
        private ScrollBar m_ScrollBar;
        private HtmlGump m_SkillsList;

        private WorldModel m_World;

        public SkillsGump()
            : base(0, 0)
        {
            IsMovable = true;

            m_World = UltimaServices.GetService<WorldModel>();

            AddControl(m_Background = new ExpandableScroll(this, 0, 0, 0, 200));
            m_Background.TitleGumpID = 0x834;

            AddControl(m_ScrollBar = new ScrollBar(this, 0));
            m_ScrollBar.IsVisible = false;

            AddControl(m_SkillsList = new HtmlGump(this, 0, 10, 20, 180, 100, 0, 1, string.Empty));
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("skills");
            base.OnInitialize();
        }

        public override void Update(double totalMS, double frameMS)
        {
            m_SkillsList.Position = new Point(26, 33);
            m_SkillsList.Width = Width - 56;
            m_SkillsList.Height = Height - 95;
            m_SkillsList.Text = buildSkillsString();
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
            m_SkillsList.Text = buildSkillsString();
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
