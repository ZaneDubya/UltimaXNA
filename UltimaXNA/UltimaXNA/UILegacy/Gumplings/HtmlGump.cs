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
using UltimaXNA.Graphics;
using UltimaXNA.Input;
using UltimaXNA.UILegacy.HTML;

namespace UltimaXNA.UILegacy.Gumplings
{
    public class HtmlGump : Control
    {
        public int ScrollX = 0, ScrollY = 0;
        ScrollBar _scrollbar;


        string _text = string.Empty;
        bool _textChanged = false;
        public string Text
        {
            get { return _text; }
            set
            {
                if (value != _text)
                {
                    _textChanged = true;
                    _text = value;
                }
            }
        }

        Texture2D _backgroundTexture;
        bool _background = false;
        public bool Background
        {
            get { return _background; }
            set { _background = value; }
        }

        bool _hasScrollbar = false;
        public bool HasScrollbar
        {
            get { return _hasScrollbar; }
            set { _hasScrollbar = value; }
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
                    if (_textRenderer != null)
                    {
                        _textRenderer.MaxWidth = ClientWidth;
                        _textChanged = true;
                    }
                }
            }
        }

        public int ClientWidth
        {
            get
            {
                if (HasScrollbar)
                    return Width - 20;
                else
                    return Width;
            }
        }

        TextRenderer _textRenderer;

        public HtmlGump(Control owner, int page)
            : base(owner, page)
        {
            _textChanged = true;
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
            Height = height;
            Size = new Point2D(width, height);
            Text = text;
            _background = (background == 1) ? true : false;
            _hasScrollbar = (scrollbar == 1) ? true : false;
            _textRenderer = new TextRenderer(text, ClientWidth, true);
        }

        public override void Update(GameTime gameTime)
        {
            _hrefOver = -1; // this value is changed every frame if we mouse over a region.

            if (_textChanged)
            {
                _textChanged = false;
                _textRenderer.Text = Text;
            }

            HandlesMouseInput = (_textRenderer.HREFRegions.Count > 0);

            if (HasScrollbar)
            {
                if (_scrollbar == null)
                    AddControl(_scrollbar = new ScrollBar(this, 0));
                _scrollbar.X = Width - 15;
                _scrollbar.Y = 0;
                _scrollbar.Width = 15;
                _scrollbar.Height = Height;
                _scrollbar.MinValue = 0;
                _scrollbar.MaxValue = _textRenderer.Height - Height;
                ScrollY = _scrollbar.Value;
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            if (_background)
            {
                if (_backgroundTexture == null)
                {
                    _backgroundTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                    _backgroundTexture.SetData<Color>(new Color[] { Color.White });
                }
                spriteBatch.Draw2D(_backgroundTexture, new Rectangle(OwnerX + Area.X, OwnerY + Area.Y, Width, Height), 0, false, false);
            }

            _textRenderer.ActiveHREF = _hrefOver;
            _textRenderer.ActiveHREF_UseDownHue = _clicked;
            _textRenderer.Draw(spriteBatch, new Rectangle(X, Y, Size.X, Size.Y), ScrollX, ScrollY);
            
            base.Draw(spriteBatch);
        }

        protected override bool _hitTest(int x, int y)
        {
            Point2D position = new Point2D(x + OwnerX + X, y + OwnerY + Y);
            if (HasScrollbar)
            {
                if (_scrollbar.HitTest(position, true) != null)
                    return true;
            }

            if (_textRenderer.HREFRegions.Count > 0)
            {
                HTMLRegion region = _textRenderer.HREFRegions.RegionfromPoint(new Point(x + ScrollX, y + ScrollY));
                if (region != null)
                {
                    _hrefOver = region.Index;
                    return true;
                }
            }
            return false;
        }

        bool _clicked = false;
        int _hrefClicked = -1;
        int _hrefOver = -1;

        protected override void mouseDown(int x, int y, MouseButton button)
        {
            _clicked = true;
            _hrefClicked = _hrefOver;
        }

        protected override void mouseUp(int x, int y, MouseButton button)
        {
            _clicked = false;
            _hrefClicked = -1;
        }

        protected override void mouseClick(int x, int y, MouseButton button)
        {
            if (_hrefOver != -1 && _hrefClicked == _hrefOver)
            {
                if (button == MouseButton.Left)
                {
                    if (_textRenderer.HREFRegions.Region(_hrefOver).HREFAttributes != null)
                        ActivateByHREF(_textRenderer.HREFRegions.Region(_hrefOver).HREFAttributes.HREF);
                }
            }
        }

        protected override void mouseOver(int x, int y)
        {

        }
    }
}
