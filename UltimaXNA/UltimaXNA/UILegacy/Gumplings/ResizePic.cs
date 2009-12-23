using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    class ResizePic : Control
    {
        Texture2D[] _bgGumps = null;
        int GumpID = 0;

        public ResizePic(Control owner, int page)
            : base(owner, page)
        {
            _bgGumps = new Texture2D[9];
            HandlesInput = true;
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

        public override void Draw(UltimaXNA.Graphics.ExtendedSpriteBatch spriteBatch)
        {
            int centerWidth = Area.Width - _bgGumps[0].Width - _bgGumps[2].Width;
            int centerHeight = Area.Height - _bgGumps[0].Height - _bgGumps[2].Height;
            int line2Y = Area.Y + _bgGumps[0].Height;
            int line3Y = Area.Y + Area.Height - _bgGumps[6].Height;

            spriteBatch.Draw(_bgGumps[0], new Vector2(Area.X, Area.Y), Color.White);
            spriteBatch.DrawTiled(_bgGumps[1], new Rectangle(Area.X + _bgGumps[0].Width, Area.Y, centerWidth, _bgGumps[0].Height), Color.White);
            spriteBatch.Draw(_bgGumps[2], new Vector2(Area.X + Area.Width - _bgGumps[2].Width, Area.Y), Color.White);

            spriteBatch.DrawTiled(_bgGumps[3], new Rectangle(Area.X, line2Y, _bgGumps[0].Width, centerHeight), Color.White);
            spriteBatch.DrawTiled(_bgGumps[4], new Rectangle(Area.X + _bgGumps[0].Width, line2Y, centerWidth, centerHeight), Color.White);
            spriteBatch.DrawTiled(_bgGumps[5], new Rectangle(Area.X + Area.Width - _bgGumps[2].Width, line2Y, _bgGumps[2].Width, centerHeight), Color.White);

            spriteBatch.Draw(_bgGumps[6], new Vector2(Area.X, line3Y), Color.White);
            spriteBatch.DrawTiled(_bgGumps[7], new Rectangle(Area.X + _bgGumps[0].Width, line3Y, centerWidth, _bgGumps[6].Height), Color.White);
            spriteBatch.Draw(_bgGumps[8], new Vector2(Area.X + Area.Width - _bgGumps[2].Width, line3Y), Color.White);

            base.Draw(spriteBatch);
        }

        bool isMoving = false; int moveOriginalX, moveOriginalY;

        public override void _mouseDown(int x, int y, int button)
        {
            x += _owner.X;
            y += _owner.Y;
            if (button == 0)
            {
                // move!
                isMoving = true;
                moveOriginalX = x;
                moveOriginalY = y;
            }
        }

        public override void _mouseUp(int x, int y, int button)
        {
            x += _owner.X;
            y += _owner.Y;
            if (button == 0)
            {
                if (isMoving == true)
                {
                    isMoving = false;
                    _owner.X += (x - moveOriginalX);
                    _owner.Y += (y - moveOriginalY);
                }
            }
        }

        public override void _mouseOver(int x, int y)
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

        public override void _mouseClick(int x, int y, int button)
        {
            if (button == 2)
            {
                _owner.Dispose();
            }
        }
    }
}
