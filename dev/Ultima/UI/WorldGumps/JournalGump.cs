/***************************************************************************
 *   JournalGump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region Usings
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class JournalGump : Gump
    {
        private ExpandableScroll m_Background;
        private List<RenderedText> m_JournalEntries;
        private ScrollBar m_ScrollBar;

        public JournalGump()
            : base(0, 0)
        {
            IsMovable = true;

            AddControl(m_Background = new ExpandableScroll(this, 0, 0, 300));
            m_Background.TitleGumpID = 0x82A;

            AddControl(m_ScrollBar = new ScrollBar(this));
            m_ScrollBar.IsVisible = false;
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("journal");

            m_JournalEntries = new List<RenderedText>();
            InitializeJournalEntries();
            PlayerState.Journaling.OnJournalEntryAdded += AddJournalEntry;
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);

            m_ScrollBar.Position = new Point(Width - 45, 35);
            m_ScrollBar.Height = Height - 100;
            CalculateScrollBarMaxValue();
            m_ScrollBar.IsVisible = m_ScrollBar.MaxValue > m_ScrollBar.MinValue;
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            base.Draw(spriteBatch, position);

            Point p = new Point(position.X + 36, position.Y + 35);
            int height = 0;
            int maxheight = m_ScrollBar.Value + m_ScrollBar.Height;

            for (int i = 0; i < m_JournalEntries.Count; i++)
            {
                if (height + m_JournalEntries[i].Height <= m_ScrollBar.Value)
                {
                    // this entry is above the renderable area.
                    height += m_JournalEntries[i].Height;
                }
                else if (height + m_JournalEntries[i].Height <= maxheight)
                {
                    int y = height - m_ScrollBar.Value;
                    if (y < 0)
                    {
                        // this entry starts above the renderable area, but exists partially within it.
                        m_JournalEntries[i].Draw(spriteBatch, new Rectangle(p.X, position.Y + 35, m_JournalEntries[i].Width, m_JournalEntries[i].Height + y), 0, -y);
                        p.Y += m_JournalEntries[i].Height + y;
                    }
                    else
                    {
                        // this entry is completely within the renderable area.
                        m_JournalEntries[i].Draw(spriteBatch, p);
                        p.Y += m_JournalEntries[i].Height;
                    }
                    height += m_JournalEntries[i].Height;
                }
                else
                {
                    int y = maxheight - height;
                    m_JournalEntries[i].Draw(spriteBatch, new Rectangle(p.X, position.Y + 35 + m_ScrollBar.Height - y, m_JournalEntries[i].Width, y), 0, 0);
                    // can't fit any more entries - so we break!
                    break;
                }
            }
        }


        private void CalculateScrollBarMaxValue()
        {
            bool maxValue = m_ScrollBar.Value == m_ScrollBar.MaxValue;

            int height = 0;
            for (int i = 0; i < m_JournalEntries.Count; i++)
            {
                height += m_JournalEntries[i].Height;
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
            }
        }

        private void AddJournalEntry(string text)
        {
            bool maxScroll = (m_ScrollBar.Value == m_ScrollBar.MaxValue);

            while (m_JournalEntries.Count > 99)
            {
                m_JournalEntries.RemoveAt(0);
            }
            m_JournalEntries.Add(new RenderedText(string.Format("<left color=50422D><span width='14'/>{0}</left><br/>", text), 200));
            m_ScrollBar.MaxValue += m_JournalEntries[m_JournalEntries.Count - 1].Height;
            if (maxScroll)
            {
                m_ScrollBar.Value = m_ScrollBar.MaxValue;
            }
        }

        private void InitializeJournalEntries()
        {
            for (int i = 0; i < PlayerState.Journaling.JournalEntries.Count; i++)
            {
                AddJournalEntry(PlayerState.Journaling.JournalEntries[i]);
            }

            m_ScrollBar.MinValue = 0;
        }
    }
}
