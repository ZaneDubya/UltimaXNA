using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Input;

namespace UltimaXNA.UILegacy.Gumplings
{
    class DropDownList : Control
    {
        int _width;
        List<string> _items;
        int Index;

        public DropDownList(Control owner, int page)
            : base(owner, page)
        {
            HandlesMouseInput = true;
            _controls = new List<Control>();
        }

        public DropDownList(Control owner, int page, int x, int y, int width, int index, string[] items)
            : this(owner, page)
        {
            buildGumpling(x, y, width, index, items);
        }

        void buildGumpling(int x, int y, int width, int index, string[] items)
        {
            Position = new Vector2(x, y);
            _items = new List<string>(items);
            _width = width;
            Index = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (_controls.Count == 0)
            {
                Size = new Vector2(_width, Data.ASCIIText.Fonts[1].Height + 8);
                _controls.Add(new ResizePic(this, 1, X, Y, 3000, Width, Height));
                _controls.Add(new TextLabelAscii(this, 1, X + 4, Y + 5, 0, 1, "Click here"));
                // _scrollBar = new ScrollBar(this, 1, 145, 5, 50, 0, 50, 0);
                ActivePage = 1;
            }
            base.Update(gameTime);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        protected override bool _hitTest(int x, int y)
        {
            if (new Rectangle(0, 0, Width, Height).Contains(new Point(x, y)))
                return true;
            else
                return false;
        }

        bool _clicked = false;
        Point _clickPosition;

        protected override void _mouseDown(int x, int y, MouseButtons button)
        {
            _clicked = true;
            _clickPosition = new Point(x, y);
        }

        protected override void _mouseUp(int x, int y, MouseButtons button)
        {
            _clicked = false;
        }

        protected override void _mouseOver(int x, int y)
        {
            if (_clicked)
            {

            }
        }
    }
}
