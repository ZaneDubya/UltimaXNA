/***************************************************************************
 *   HtmlGump.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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
using UltimaXNA.Rendering;
using InterXLib.Input.Windows;
using UltimaXNA.UltimaGUI.HTML;
using UltimaXNA.UltimaGUI;

namespace UltimaXNA.UltimaGUI.Controls
{
    public class HtmlGump : Control
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

        Texture2D m_backgroundTexture;
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
                    if (m_textRenderer != null)
                    {
                        m_textRenderer.MaxWidth = ContentWidth;
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

        UltimaGUI.TextRenderer m_textRenderer;

        public HtmlGump(Control owner, int page)
            : base(owner, page)
        {
            m_textChanged = true;
        }

        public HtmlGump(Control owner, int page, string[] arguements, string[] lines)
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

            buildGumpling(x, y, width, height, background, scrollbar, lines[textIndex]);
        }

        public HtmlGump(Control owner, int page, int x, int y, int width, int height, int background, int scrollbar, string text)
            : this(owner, page)
        {
            buildGumpling(x, y, width, height, background, scrollbar, text);
        }

        void buildGumpling(int x, int y, int width, int height, int background, int scrollbar, string text)
        {
            Position = new Point2D(x, y);
            Width = width;
            Size = new Point2D(width, height);
            Text = text;
            m_background = (background == 1) ? true : false;
            m_hasScrollbar = (scrollbar == 1) ? true : false;
            m_textRenderer = new TextRenderer(text, Width, true);
            Height = m_textRenderer.Height;
        }

        public override void Update(GameTime gameTime)
        {
            m_hrefOver = -1; // this value is changed every frame if we mouse over a region.

            if (m_textChanged)
            {
                m_textChanged = false;
                m_textRenderer.Text = Text;
            }

            HandlesMouseInput = (m_textRenderer.HREFRegions.Count > 0);

            if (HasScrollbar)
            {
                if (m_scrollbar == null)
                    AddControl(m_scrollbar = new ScrollBar(this, 0));
                m_scrollbar.X = Width - 15;
                m_scrollbar.Y = 0;
                m_scrollbar.Width = 15;
                m_scrollbar.Height = Height;
                m_scrollbar.MinValue = 0;
                m_scrollbar.MaxValue = m_textRenderer.Height - Height;
                ScrollY = m_scrollbar.Value;
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            if (m_background)
            {
                if (m_backgroundTexture == null)
                {
                    m_backgroundTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                    m_backgroundTexture.SetData<Color>(new Color[] { Color.White });
                }
                spriteBatch.Draw2D(m_backgroundTexture, new Rectangle(OwnerX + Area.X, OwnerY + Area.Y, Width, Height), 0, false, false);
            }

            m_textRenderer.ActiveHREF = m_hrefOver;
            m_textRenderer.ActiveHREF_UseDownHue = m_clicked;
            m_textRenderer.Draw(spriteBatch, new Rectangle(X, Y, Size.X, Size.Y), ScrollX, ScrollY);
            
            base.Draw(spriteBatch);
        }

        protected override bool m_hitTest(int x, int y)
        {
            Point2D position = new Point2D(x + OwnerX + X, y + OwnerY + Y);
            if (HasScrollbar)
            {
                if (m_scrollbar.HitTest(position, true) != null)
                    return true;
            }

            if (m_textRenderer.HREFRegions.Count > 0)
            {
                HTMLRegion region = m_textRenderer.HREFRegions.RegionfromPoint(new Point(x + ScrollX, y + ScrollY));
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

        protected override void mouseDown(int x, int y, MouseButton button)
        {
            m_clicked = true;
            m_hrefClicked = m_hrefOver;
        }

        protected override void mouseUp(int x, int y, MouseButton button)
        {
            m_clicked = false;
            m_hrefClicked = -1;
        }

        protected override void mouseClick(int x, int y, MouseButton button)
        {
            if (m_hrefOver != -1 && m_hrefClicked == m_hrefOver)
            {
                if (button == MouseButton.Left)
                {
                    if (m_textRenderer.HREFRegions.Region(m_hrefOver).HREFAttributes != null)
                        ActivateByHREF(m_textRenderer.HREFRegions.Region(m_hrefOver).HREFAttributes.HREF);
                }
            }
        }

        protected override void mouseOver(int x, int y)
        {

        }
    }
}
