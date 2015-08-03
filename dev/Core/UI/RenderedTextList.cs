/***************************************************************************
 *   RenderedTextList.cs
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
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Core.UI
{
    /// <summary>
    /// A one dimensional list of rendered text objects which can be scrolled (up and down) and
    /// only display within a designated window.
    /// </summary>
    class RenderedTextList : AControl
    {
        private List<RenderedText> m_JournalEntries;
        private IScrollBar m_ScrollBar;

        /// <summary>
        /// Creates a RenderedTextList.
        /// Note that the scrollBarControl must be created and added to the parent gump before passing it as a param.
        /// </summary>
        public RenderedTextList(AControl parent, int x, int y, int width, int height, IScrollBar scrollBarControl)
            : base(parent)
        {
            m_ScrollBar = scrollBarControl;
            m_ScrollBar.IsVisible = false;

            Position = new Point(x, y);
            Width = width;
            Height = height;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            m_JournalEntries = new List<RenderedText>();
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            base.Draw(spriteBatch, position);

            Point p = new Point(position.X, position.Y);
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
                        m_JournalEntries[i].Draw(spriteBatch, new Rectangle(p.X, position.Y, m_JournalEntries[i].Width, m_JournalEntries[i].Height + y), 0, -y);
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
                    m_JournalEntries[i].Draw(spriteBatch, new Rectangle(p.X, position.Y + m_ScrollBar.Height - y, m_JournalEntries[i].Width, y), 0, 0);
                    // can't fit any more entries - so we break!
                    break;
                }
            }
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);

            m_ScrollBar.Position = new Point(X + Width - 14, Y);
            m_ScrollBar.Height = Height;
            CalculateScrollBarMaxValue();
            m_ScrollBar.IsVisible = m_ScrollBar.MaxValue > m_ScrollBar.MinValue;
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

        public void AddEntry(string text)
        {
            bool maxScroll = (m_ScrollBar.Value == m_ScrollBar.MaxValue);

            while (m_JournalEntries.Count > 99)
            {
                m_JournalEntries.RemoveAt(0);
            }
            m_JournalEntries.Add(new RenderedText(text, Width - 14));
            m_ScrollBar.MaxValue += m_JournalEntries[m_JournalEntries.Count - 1].Height;
            if (maxScroll)
            {
                m_ScrollBar.Value = m_ScrollBar.MaxValue;
            }
        }
    }
}
