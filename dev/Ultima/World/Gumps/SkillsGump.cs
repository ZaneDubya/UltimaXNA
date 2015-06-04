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
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.World.Gumps
{
    class SkillsGump : Gump
    {
        // Private variables
        private ExpandableScroll m_Background;
        private HtmlGumpling m_SkillsHtml;
        // Services
        private WorldModel m_World;

        public SkillsGump()
            : base(0, 0)
        {
            IsMovable = true;

            m_World = UltimaServices.GetService<WorldModel>();

            AddControl(m_Background = new ExpandableScroll(this, 0, 0, 0, 200));
            m_Background.TitleGumpID = 0x834;

            AddControl(m_SkillsHtml = new HtmlGumpling(this, 0, 36, 35, 230, Height - 100, 0, 1, string.Empty));
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("skills");
            // m_SkillsList = new List<RenderedText>();
            InitializeSkillsList();
            PlayerState.Skills.OnSkillChanged += OnSkillChanged;
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);

            m_SkillsHtml.Height = Height - 100;
            // m_ScrollBar.Position = new Point(Width - 45, 35);
            // m_ScrollBar.Height = Height - 100;
            // CalculateScrollBarMaxValue();
            // m_ScrollBar.IsVisible = m_ScrollBar.MaxValue > m_ScrollBar.MinValue;
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            base.Draw(spriteBatch, position);

            /*Point p = new Point(position.X + 36, position.Y + 35);
            int height = 0;
            int maxheight = m_ScrollBar.Value + m_ScrollBar.Height;

            for (int i = 0; i < m_SkillsList.Count; i++)
            {
                if (height + m_SkillsList[i].Height <= m_ScrollBar.Value)
                {
                    // this entry is above the renderable area.
                    height += m_SkillsList[i].Height;
                }
                else if (height + m_SkillsList[i].Height <= maxheight)
                {
                    int y = height - m_ScrollBar.Value;
                    if (y < 0)
                    {
                        // this entry starts above the renderable area, but exists partially within it.
                        m_SkillsList[i].Draw(spriteBatch, new Rectangle(p.X, position.Y + 35, m_SkillsList[i].Width, m_SkillsList[i].Height + y), 0, -y);
                        p.Y += m_SkillsList[i].Height + y;
                    }
                    else
                    {
                        // this entry is completely within the renderable area.
                        m_SkillsList[i].Draw(spriteBatch, p);
                        p.Y += m_SkillsList[i].Height;
                    }
                    height += m_SkillsList[i].Height;
                }
                else
                {
                    int y = maxheight - height;
                    m_SkillsList[i].Draw(spriteBatch, new Rectangle(p.X, position.Y + 35 + m_ScrollBar.Height - y, m_SkillsList[i].Width, y), 0, 0);
                    // can't fit any more entries - so we break!
                    break;
                }
            }*/
        }

        private void CalculateScrollBarMaxValue()
        {
            /*bool maxValue = m_ScrollBar.Value == m_ScrollBar.MaxValue;

            int height = 0;
            for (int i = 0; i < m_SkillsList.Count; i++)
            {
                height += m_SkillsList[i].Height;
            }

            height -= m_ScrollBar.Height;

            if (height > 0)
            {
                m_ScrollBar.MaxValue = height;
                if (maxValue)
                    m_ScrollBar.Value = m_ScrollBar.MaxValue;
            }
            else
            {
                m_ScrollBar.MaxValue = 0;
                m_ScrollBar.Value = 0;
            }*/
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
            else if (href.Substring(0, 10) == "skilllock=")
            {
                int skillIndex;
                if (!int.TryParse(href.Substring(10), out skillIndex))
                    return;
                m_World.Interaction.ChangeSkillLock(PlayerState.Skills.SkillEntryByIndex(skillIndex));
            }
            base.ActivateByHREF(href);
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

        }

        // 0 = skill index, 1 = skill name
        const string kSkillName_HasUseButton = "<left><a href='skill={0}' color='#5b4f29' hovercolor='#857951' activecolor='#402708' text-decoration=none>" +
                        "<gumpimg src='2103' hoversrc='2104' activesrc='2103'/><span width='2'/>{1}</a></left>";
        const string kSkillName_NoUseButton = "<left><span width='14'/><medium color=#50422D>{1}</medium></left>";
        // 0 = skill value
        static string[] kSkillValues = new string[3] {
            "<right>{0:0.0}<a href='skilllock={1}'><gumpimg src='2436'/></a>  </right><br/>",
            "<right>{0:0.0}<a href='skilllock={1}'><gumpimg src='2438'/></a>  </right><br/>",
            "<right>{0:0.0}<a href='skilllock={1}'><gumpimg src='2092'/></a>  </right><br/>" };
    }
}
