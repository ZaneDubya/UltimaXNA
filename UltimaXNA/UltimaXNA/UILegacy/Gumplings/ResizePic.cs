using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Input;

namespace UltimaXNA.UILegacy.Gumplings
{
    class ResizePic : Control
    {
        public bool CloseOnRightClick = false;

        Texture2D[] _bgGumps = null;
        int GumpID = 0;

        public ResizePic(Control owner, int page)
            : base(owner, page)
        {
            _bgGumps = new Texture2D[9];
            HandlesMouseInput = true;
        }

        public ResizePic(Control owner, int page, string[] arguements)
            : this(owner, page)
        {
            int x, y, gumpID, width, height;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            gumpID = Int32.Parse(arguements[3]);
            width = Int32.Parse(arguements[4]);
            height = Int32.Parse(arguements[5]);
            buildGumpling(x, y, gumpID, width, height);
        }

        public ResizePic(Control owner, int page, int x, int y, int gumpID, int width, int height)
            : this(owner, page)
        {
            buildGumpling(x, y, gumpID, width, height);
        }

        public ResizePic(Control owner, Control c)
            : this(owner, c.Page)
        {
            buildGumpling(c.X - 4, c.Y - 4, 9350, c.Width + 8, c.Height + 8);
        }

        void buildGumpling(int x, int y, int gumpID, int width, int height)
        {
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
            GumpID = gumpID;
        }

        public override void Update(GameTime gameTime)
        {
            if (_bgGumps[0] == null)
            {
                for (int i = 0; i < 9; i++)
                {
                    _bgGumps[i] = Data.Gumps.GetGumpXNA(GumpID + i);
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            int centerWidth = Area.Width - _bgGumps[0].Width - _bgGumps[2].Width;
            int centerHeight = Area.Height - _bgGumps[0].Height - _bgGumps[2].Height;
            int line2Y = Area.Y + _bgGumps[0].Height;
            int line3Y = Area.Y + Area.Height - _bgGumps[6].Height;

            spriteBatch.Draw(_bgGumps[0], new Vector2(Area.X, Area.Y), 0, false);
            spriteBatch.DrawSimpleTiled(_bgGumps[1], new Rectangle(Area.X + _bgGumps[0].Width, Area.Y, centerWidth, _bgGumps[0].Height), 0, false);
            spriteBatch.Draw(_bgGumps[2], new Vector2(Area.X + Area.Width - _bgGumps[2].Width, Area.Y), 0, false);

            spriteBatch.DrawSimpleTiled(_bgGumps[3], new Rectangle(Area.X, line2Y, _bgGumps[0].Width, centerHeight), 0, false);
            spriteBatch.DrawSimpleTiled(_bgGumps[4], new Rectangle(Area.X + _bgGumps[0].Width, line2Y, centerWidth, centerHeight), 0, false);
            spriteBatch.DrawSimpleTiled(_bgGumps[5], new Rectangle(Area.X + Area.Width - _bgGumps[2].Width, line2Y, _bgGumps[2].Width, centerHeight), 0, false);

            spriteBatch.Draw(_bgGumps[6], new Vector2(Area.X, line3Y), 0, false);
            spriteBatch.DrawSimpleTiled(_bgGumps[7], new Rectangle(Area.X + _bgGumps[0].Width, line3Y, centerWidth, _bgGumps[6].Height), 0, false);
            spriteBatch.Draw(_bgGumps[8], new Vector2(Area.X + Area.Width - _bgGumps[2].Width, line3Y), 0, false);

            base.Draw(spriteBatch);
        }

        bool isMoving = false; int moveOriginalX, moveOriginalY;

        protected override void _mouseDown(int x, int y, MouseButtons button)
        {
            x += _owner.X;
            y += _owner.Y;
            if (button == MouseButtons.LeftButton)
            {
                // move!
                isMoving = true;
                moveOriginalX = x;
                moveOriginalY = y;
            }
        }

        protected override void _mouseUp(int x, int y, MouseButtons button)
        {
            x += _owner.X;
            y += _owner.Y;
            if (button == MouseButtons.LeftButton)
            {
                if (isMoving == true)
                {
                    isMoving = false;
                    _owner.X += (x - moveOriginalX);
                    _owner.Y += (y - moveOriginalY);
                }
            }
        }

        protected override void _mouseOver(int x, int y)
        {
            x += _owner.X;
            y += _owner.Y;
            if (isMoving == true)
            {
                _owner.X += (x - moveOriginalX);
                _owner.Y += (y - moveOriginalY);
                moveOriginalX = x;
                moveOriginalY = y;
            }
        }

        protected override void _mouseClick(int x, int y, MouseButtons button)
        {
            if (button == MouseButtons.RightButton)
            {
                if (CloseOnRightClick)
                    _owner.Dispose();
            }
        }
    }
}
