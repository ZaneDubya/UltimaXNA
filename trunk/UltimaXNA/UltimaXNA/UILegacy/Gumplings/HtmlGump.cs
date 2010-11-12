using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Input;

namespace UltimaXNA.UILegacy.Gumplings
{
    class HtmlGump : Control
    {
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
        bool _background = false;
        bool _scrollbar = false;
        int _width, _height;

        Texture2D _texture = null;
        HREFRegions _hrefRegions = null;

        public HtmlGump(Control owner, int page)
            : base(owner, page)
        {

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
            _width = width;
            _height = height;
            Size = new Point2D(width, height);
            Text = text;
            _background = (background == 1) ? true : false;
            _scrollbar = (scrollbar == 1) ? true : false;
        }

        public override void Update(GameTime gameTime)
        {
            _hrefOver = -1; // this value is changed every frame if we mouse over a region.
            if (_textChanged || _texture == null)
            {
                _textChanged = false;
                _texture = Data.UniText.GetTextureHTML(Text, _width, _height, ref _hrefRegions);
                Size = new Point2D(_texture.Width, _texture.Height);
                if (_hrefRegions.Count > 0)
                    HandlesMouseInput = true;
                else
                    HandlesMouseInput = false;
            }
            base.Update(gameTime);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            spriteBatch.Draw2D(_texture, Position, 0, false);
            
            foreach (HREFRegion r in _hrefRegions.Regions)
            {
                if (r.Index == _hrefOver)
                {
                    if (_clicked)
                        spriteBatch.Draw2D(_texture, new Point2D(X + r.Area.X, Y + r.Area.Y), r.Area, r.Data.DownHue, false);
                    else
                        spriteBatch.Draw2D(_texture, new Point2D(X + r.Area.X, Y + r.Area.Y), r.Area, r.Data.OverHue, false);
                }
                else
                {
                    spriteBatch.Draw2D(_texture, new Point2D(X + r.Area.X, Y + r.Area.Y), r.Area, r.Data.UpHue, true);
                }
            }
            
            base.Draw(spriteBatch);
        }

        protected override bool _hitTest(int x, int y)
        {
            if (_hrefRegions.Count > 0 && _hrefRegions.RegionfromPoint(new Point(x, y)) != null)
            {
                _hrefOver = _hrefRegions.RegionfromPoint(new Point(x, y)).Index;
                return true;
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

        protected override void mouseOver(int x, int y)
        {

        }

        protected override void mouseClick(int x, int y, MouseButton button)
        {
            if (_hrefOver != -1 && _hrefClicked == _hrefOver)
            {
                if (button == MouseButton.Left)
                {
                    ActivateByHREF(_hrefRegions.Region(_hrefOver).Data.HREF);
                }
            }
        }
    }
}
