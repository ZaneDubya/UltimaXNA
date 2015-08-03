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
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.UI.HTML;
using UltimaXNA.Core.Diagnostics.Tracing;
#endregion

namespace UltimaXNA.Core.UI
{
    /// <summary>
    /// A one dimensional list of rendered text objects which can be scrolled (up and down) and
    /// only display within a designated window.
    /// </summary>
    class RenderedTextList : AControl
    {
        private List<RenderedText> m_Entries;
        private IScrollBar m_ScrollBar;

        private bool m_IsMouseDown = false;
        private int m_MouseDownHREF = -1;
        private int m_MouseDownText = -1;
        private int m_MouseOverHREF = -1;
        private int m_MouseOverText = -1;

        /// <summary>
        /// Creates a RenderedTextList.
        /// Note that the scrollBarControl must be created and added to the parent gump before passing it as a param.
        /// </summary>
        public RenderedTextList(AControl parent, int x, int y, int width, int height, IScrollBar scrollBarControl)
            : base(parent)
        {
            m_ScrollBar = scrollBarControl;
            m_ScrollBar.IsVisible = false;

            HandlesMouseInput = true;

            Position = new Point(x, y);
            Width = width;
            Height = height;
            m_Entries = new List<RenderedText>();
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            base.Draw(spriteBatch, position);

            Point p = new Point(position.X, position.Y);
            int height = 0;
            int maxheight = m_ScrollBar.Value + m_ScrollBar.Height;

            for (int i = 0; i < m_Entries.Count; i++)
            {
                if (height + m_Entries[i].Height <= m_ScrollBar.Value)
                {
                    // this entry is above the renderable area.
                    height += m_Entries[i].Height;
                }
                else if (height + m_Entries[i].Height <= maxheight)
                {
                    int y = height - m_ScrollBar.Value;
                    if (y < 0)
                    {
                        // this entry starts above the renderable area, but exists partially within it.
                        m_Entries[i].Draw(spriteBatch, new Rectangle(p.X, position.Y, m_Entries[i].Width, m_Entries[i].Height + y), 0, -y);
                        p.Y += m_Entries[i].Height + y;
                    }
                    else
                    {
                        // this entry is completely within the renderable area.
                        m_Entries[i].Draw(spriteBatch, p);
                        p.Y += m_Entries[i].Height;
                    }
                    height += m_Entries[i].Height;
                }
                else
                {
                    int y = maxheight - height;
                    m_Entries[i].Draw(spriteBatch, new Rectangle(p.X, position.Y + m_ScrollBar.Height - y, m_Entries[i].Width, y), 0, 0);
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

        protected override bool IsPointWithinControl(int x, int y)
        {
            m_MouseOverText = -1; // this value is changed every frame if we mouse over a region.
            m_MouseOverHREF = -1; // this value is changed every frame if we mouse over a region.

            int height = 0;
            for (int i = 0; i < m_Entries.Count; i++)
            {
                RenderedText rendered = m_Entries[i];
                if (rendered.Regions.Count > 0)
                {
                    Region region = rendered.Regions.RegionfromPoint(new Point(x, y - height + m_ScrollBar.Value));
                    if (region != null)
                    {
                        m_MouseOverText = i;
                        m_MouseOverHREF = region.Index;
                        return true;
                    }
                }
                height += rendered.Height;
            }
            return false;
        }

        protected override void OnMouseDown(int x, int y, MouseButton button)
        {
            m_IsMouseDown = true;
            m_MouseDownText = m_MouseOverText;
            m_MouseDownHREF = m_MouseOverHREF;
        }

        protected override void OnMouseUp(int x, int y, MouseButton button)
        {
            m_IsMouseDown = false;
            m_MouseDownText = -1;
            m_MouseDownHREF = -1;
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            if (m_MouseOverText != -1 && m_MouseOverHREF != -1 && m_MouseDownText == m_MouseOverText && m_MouseDownHREF == m_MouseOverHREF)
            {
                if (button == MouseButton.Left)
                {
                    if (m_Entries[m_MouseOverText].Regions.Region(m_MouseOverHREF).HREF != null)
                        ActivateByHREF(m_Entries[m_MouseOverText].Regions.Region(m_MouseOverHREF).HREF.HREF);
                }
            }
        }

        protected override void OnMouseOver(int x, int y)
        {
            if (m_IsMouseDown && m_MouseDownText != -1 && m_MouseDownHREF != -1 && m_MouseDownHREF != m_MouseOverHREF)
            {
                // no need for dragging in this control - yet. Wouldn't be too hard to add. See HtmlGumpling for how to do it.
                /*if (OnDragHRef != null)
                    OnDragHRef(m_RenderedText.Regions.Region(m_MouseDownHREF).HREF.HREF);*/
            }
        }

        private void CalculateScrollBarMaxValue()
        {
            bool maxValue = m_ScrollBar.Value == m_ScrollBar.MaxValue;

            int height = 0;
            for (int i = 0; i < m_Entries.Count; i++)
            {
                height += m_Entries[i].Height;
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

            while (m_Entries.Count > 99)
            {
                m_Entries.RemoveAt(0);
            }
            m_Entries.Add(new RenderedText(text, Width - 18));
            m_ScrollBar.MaxValue += m_Entries[m_Entries.Count - 1].Height;
            if (maxScroll)
            {
                m_ScrollBar.Value = m_ScrollBar.MaxValue;
            }
        }

        public void UpdateEntry(int index, string text)
        {
            if (index < 0 || index >= m_Entries.Count)
            {
                Tracer.Error(string.Format("Bad index in RenderedTextList.UpdateEntry: {0}", index.ToString()));
                return;
            }

            m_Entries[index].Text = text;
            CalculateScrollBarMaxValue();
        }
    }
}
