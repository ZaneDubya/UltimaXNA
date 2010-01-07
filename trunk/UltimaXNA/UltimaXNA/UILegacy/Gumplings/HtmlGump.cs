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
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
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
                _texture = Data.UniText.GetTexture(Text, Area.Width, Area.Height, ref _hrefRegions);
                if (_hrefRegions.Count > 0)
                    HandlesMouseInput = true;
                else
                    HandlesMouseInput = false;
            }
            base.Update(gameTime);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, new Vector2(Area.X, Area.Y), 0, false);
            
            foreach (HREFRegion r in _hrefRegions.Regions)
            {
                if (r.Index == _hrefOver)
                {
                    if (_clicked)
                        spriteBatch.Draw(_texture, new Vector2(Area.X + r.Area.X, Area.Y + r.Area.Y), r.Area, r.DownHue, false);
                    else
                        spriteBatch.Draw(_texture, new Vector2(Area.X + r.Area.X, Area.Y + r.Area.Y), r.Area, r.OverHue, false);
                }
                else
                {
                    spriteBatch.Draw(_texture, new Vector2(Area.X + r.Area.X, Area.Y + r.Area.Y), r.Area, r.UpHue, true);
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

        protected override void _mouseDown(int x, int y, MouseButtons button)
        {
            _clicked = true;
            _hrefClicked = _hrefOver;
        }

        protected override void _mouseUp(int x, int y, MouseButtons button)
        {
            _clicked = false;
            _hrefClicked = -1;
        }

        protected override void _mouseOver(int x, int y)
        {

        }

        protected override void _mouseClick(int x, int y, MouseButtons button)
        {
            if (_hrefOver != -1 && _hrefClicked == _hrefOver)
            {
                if (button == MouseButtons.LeftButton)
                {
                    ActivateByHREF(_hrefRegions.Region(_hrefOver).HREF);
                }
            }
        }
    }
}
