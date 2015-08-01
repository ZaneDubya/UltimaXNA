/***************************************************************************
 *   SkillsGump.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using System.Text;
using UltimaXNA.Core.Input;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class SkillsGump : Gump
    {
        // Private variables
        private ExpandableScroll m_Background;
        private HtmlGumpling m_SkillsHtml;
        private bool m_MustUpdateSkills = true;
        // Services
        private WorldModel m_World;

        public SkillsGump()
            : base(0, 0)
        {
            IsMoveable = true;

            m_World = ServiceRegistry.GetService<WorldModel>();

            AddControl(m_Background = new ExpandableScroll(this, 0, 0, 200));
            m_Background.TitleGumpID = 0x834;

            AddControl(m_SkillsHtml = new HtmlGumpling(this, 36, 35, 230, 200 - 100, 0, 2, string.Empty));
            m_SkillsHtml.OnDragHRef += OnSkillDrag;
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("skills");
            PlayerState.Skills.OnSkillChanged += OnSkillChanged;
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            if (m_MustUpdateSkills)
            {
                InitializeSkillsList();
                m_MustUpdateSkills = false;
            }
            m_SkillsHtml.Height = Height - 100;
        }

        public override void ActivateByHREF(string href)
        {
            if (href.Substring(0, 6) == "skill=" || href.Substring(0, 9) == "skillbtn=")
            {
                int skillIndex;
                if (!int.TryParse(href.Substring(href.IndexOf('=') + 1), out skillIndex))
                        return;
                m_World.Interaction.UseSkill(skillIndex);
            }
            else if (href.Substring(0, 10) == "skilllock=")
            {
                int skillIndex;
                if (!int.TryParse(href.Substring(10), out skillIndex))
                    return;
                m_World.Interaction.ChangeSkillLock(PlayerState.Skills.SkillEntryByIndex(skillIndex));
            }
        }

        private void InitializeSkillsList()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<int, SkillEntry> pair in PlayerState.Skills.List)
            {
                SkillEntry skill = pair.Value;
                sb.Append(string.Format(skill.HasUseButton ? kSkillName_HasUseButton : kSkillName_NoUseButton, skill.Index, skill.Name));
                sb.Append(string.Format(kSkillValues[skill.LockType], skill.Value, skill.Index));
            }
            m_SkillsHtml.Text = sb.ToString();
        }

        private void OnSkillChanged(SkillEntry entry)
        {
            m_MustUpdateSkills = true;
        }

        private void OnSkillDrag(string href)
        {
            if (href.Length >= 9 && href.Substring(0, 9) == "skillbtn=")
            {
                int skillIndex;
                if (!int.TryParse(href.Substring(9), out skillIndex))
                    return;
                SkillEntry skill = PlayerState.Skills.SkillEntryByIndex(skillIndex);
                InputManager input = ServiceRegistry.GetService<InputManager>();
                UseSkillButtonGump gump = new UseSkillButtonGump(skill);
                UserInterface.AddControl(gump, input.MousePosition.X - 60, input.MousePosition.Y - 20);
                UserInterface.AttemptDragControl(gump, input.MousePosition, true);
            }
        }

        // 0 = skill index, 1 = skill name
        const string kSkillName_HasUseButton = 
            "<left><a href='skillbtn={0}'><gumpimg src='2103' hoversrc='2104' activesrc='2103'/><span width='2'/></a>" +
            "<a href='skill={0}' color='#5b4f29' hovercolor='#857951' activecolor='#402708' style='text-decoration=none'>{1}</a></left>";
        const string kSkillName_NoUseButton = "<left><span width='13'/><medium color=#50422D>{1}</medium></left>";
        // 0 = skill value
        static string[] kSkillValues = new string[3] {
            "<right><medium color=#50422D>{0:0.0}</medium><a href='skilllock={1}'><gumpimg src='2436'/></a><span width='2'/></right><br/>",
            "<right><medium color=#50422D>{0:0.0}</medium><a href='skilllock={1}'><gumpimg src='2438'/></a><span width='2'/></right><br/>",
            "<right><medium color=#50422D>{0:0.0}</medium><a href='skilllock={1}'><gumpimg src='2092'/></a><span width='2'/></right><br/>" };
    }
}
