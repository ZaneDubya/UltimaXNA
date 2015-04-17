/***************************************************************************
 *   HtmlGump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Ultima.UI.HTML;
using UltimaXNA.Ultima.UI;

namespace UltimaXNA.Ultima.UI.Controls
{
    public class HtmlGump : AControl
    {
        public int ScrollX = 0, ScrollY = 0;
        ScrollBar m_scrollbar;

        string m_text = string.Empty;
        bool m_textChanged = false;
        public string Text
        {
            get { return m_text; }
            set
            {
                if (value != m_text)
                {
                    m_textChanged = true;
                    m_text = value;
                }
            }
        }

        public int Hue
        {
            get
            {
                if (m_RenderedText == null)
                    return 0;
                return m_RenderedText.Hue;
            }
            set
            {
                if (m_RenderedText != null)
                    m_RenderedText.Hue = value;
            }
        }

        bool m_background = false;
        public bool Background
        {
            get { return m_background; }
            set { m_background = value; }
        }

        bool m_hasScrollbar = false;
        public bool HasScrollbar
        {
            get { return m_hasScrollbar; }
            set { m_hasScrollbar = value; }
        }

        public override int Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                if (value != base.Width)
                {
                    base.Width = value;
                    if (m_RenderedText != null)
                    {
                        m_RenderedText.MaxWidth = ContentWidth;
                        m_textChanged = true;
                    }
                }
            }
        }

        public override int Height
        {
            get
            {
                return base.Height;
            }
            set
            {
                base.Height = value;
            }
        }

        public int ContentWidth
        {
            get
            {
                if (HasScrollbar)
                    return base.Width - 20;
                else
                    return base.Width;
            }
        }

        RenderedText m_RenderedText;

        public HtmlGump(AControl owner, int page)
            : base(owner, page)
        {
            m_textChanged = true;
        }

        public HtmlGump(AControl owner, int page, string[] arguements, string[] lines)
            : this(owner, page)
        {
            int x, y, width, height, textIndex, background, scrollbar;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            width = Int32.Parse(arguements[3]);
            height = Int32.Parse(arguements[4]);
            textIndex = Int32.Parse(arguements[5]);
            background = Int32.Parse(arguements[6]);
            scrollbar = Int32.Parse(arguements[7]);

            buildGumpling(x, y, width, height, background, scrollbar, "<font color=#000>" + lines[textIndex]);
        }

        public HtmlGump(AControl owner, int page, int x, int y, int width, int height, int background, int scrollbar, string text)
            : this(owner, page)
        {
            buildGumpling(x, y, width, height, background, scrollbar, text);
        }

        void buildGumpling(int x, int y, int width, int height, int background, int scrollbar, string text)
        {
            Position = new Point(x, y);
            Width = width;
            Size = new Point(width, height);
            Text = text;
            m_background = (background == 1) ? true : false;
            m_hasScrollbar = (scrollbar == 1) ? true : false;
            m_RenderedText = new RenderedText(text, Width - (HasScrollbar ? 15 : 0) - (Background ? 8 : 0));

            if (Background)
            {
                this.AddControl(new ResizePic(this, 0, 0, 0, 0x2486, Width - (HasScrollbar ? 15 : 0), Height));
            }
        }

        public override void Update(double totalMS, double frameMS)
        {
            m_hrefOver = -1; // this value is changed every frame if we mouse over a region.

            if (m_textChanged)
            {
                m_textChanged = false;
                m_RenderedText.Text = Text;
            }

            HandlesMouseInput = (m_RenderedText.Regions.Count > 0);

            if (HasScrollbar)
            {
                if (m_scrollbar == null)
                {
                    AddControl(m_scrollbar = new ScrollBar(this, 0));
                    m_scrollbar.X = Width - 15;
                    m_scrollbar.Y = 0;
                    m_scrollbar.Width = 15;
                    m_scrollbar.Height = Height;
                    m_scrollbar.MinValue = 0;
                    m_scrollbar.MaxValue = m_RenderedText.Height - Height + (Background ? 8 : 0);
                }
                ScrollY = m_scrollbar.Value;
            }

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            base.Draw(spriteBatch);

            m_RenderedText.ActiveRegion = m_hrefOver;
            m_RenderedText.ActiveRegion_UseDownHue = m_clicked;
            m_RenderedText.Draw(spriteBatch, 
                new Rectangle(X + (Background ? 4 : 0), Y + (Background ? 4 : 0),
                    Size.X - (Background ? 8 : 0), Size.Y - (Background ? 8 : 0)), ScrollX, ScrollY);
        }

        protected override bool InternalHitTest(int x, int y)
        {
            Point position = new Point(x + OwnerX + X, y + OwnerY + Y);
            if (HasScrollbar)
            {
                if (m_scrollbar.HitTest(position, true) != null)
                    return true;
            }

            if (m_RenderedText.Regions.Count > 0)
            {
                Region region = m_RenderedText.Regions.RegionfromPoint(new Point(x + ScrollX, y + ScrollY));
                if (region != null)
                {
                    m_hrefOver = region.Index;
                    return true;
                }
            }
            return false;
        }

        bool m_clicked = false;
        int m_hrefClicked = -1;
        int m_hrefOver = -1;

        protected override void OnMouseDown(int x, int y, MouseButton button)
        {
            m_clicked = true;
            m_hrefClicked = m_hrefOver;
        }

        protected override void OnMouseUp(int x, int y, MouseButton button)
        {
            m_clicked = false;
            m_hrefClicked = -1;
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            if (m_hrefOver != -1 && m_hrefClicked == m_hrefOver)
            {
                if (button == MouseButton.Left)
                {
                    if (m_RenderedText.Regions.Region(m_hrefOver).HREFAttributes != null)
                        ActivateByHREF(m_RenderedText.Regions.Region(m_hrefOver).HREFAttributes.HREF);
                }
            }
        }

        protected override void OnMouseOver(int x, int y)
        {

        }
    }
}
