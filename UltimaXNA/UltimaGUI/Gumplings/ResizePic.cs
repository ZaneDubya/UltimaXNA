/***************************************************************************
 *   ResizePic.cs
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
using UltimaXNA.Interface.Graphics;
using UltimaXNA.Interface.Input;
using UltimaXNA.Interface.GUI;

namespace UltimaXNA.UltimaGUI.Gumplings
{
    public class ResizePic : Control
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
            MakeDragger(_owner);
            Position = new Point2D(x, y);
            Size = new Point2D(width, height);
            GumpID = gumpID;
        }

        public override void Update(GameTime gameTime)
        {
            if (_bgGumps[0] == null)
            {
                for (int i = 0; i < 9; i++)
                {
                    _bgGumps[i] = UltimaData.Gumps.GetGumpXNA(GumpID + i);
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            int centerWidth = Width - _bgGumps[0].Width - _bgGumps[2].Width;
            int centerHeight = Height - _bgGumps[0].Height - _bgGumps[2].Height;
            int line2Y = Y + _bgGumps[0].Height;
            int line3Y = Y + Height - _bgGumps[6].Height;

            spriteBatch.Draw2D(_bgGumps[0], new Point2D(X, Y), 0, false, false);
            spriteBatch.Draw2DTiled(_bgGumps[1], new Rectangle(X + _bgGumps[0].Width, Y, centerWidth, _bgGumps[0].Height), 0, false, false);
            spriteBatch.Draw2D(_bgGumps[2], new Point2D(X + Width - _bgGumps[2].Width, Y), 0, false, false);

            spriteBatch.Draw2DTiled(_bgGumps[3], new Rectangle(X, line2Y, _bgGumps[0].Width, centerHeight), 0, false, false);
            spriteBatch.Draw2DTiled(_bgGumps[4], new Rectangle(X + _bgGumps[0].Width, line2Y, centerWidth, centerHeight), 0, false, false);
            spriteBatch.Draw2DTiled(_bgGumps[5], new Rectangle(X + Width - _bgGumps[2].Width, line2Y, _bgGumps[2].Width, centerHeight), 0, false, false);

            spriteBatch.Draw2D(_bgGumps[6], new Point2D(X, line3Y), 0, false, false);
            spriteBatch.Draw2DTiled(_bgGumps[7], new Rectangle(X + _bgGumps[0].Width, line3Y, centerWidth, _bgGumps[6].Height), 0, false, false);
            spriteBatch.Draw2D(_bgGumps[8], new Point2D(X + Width - _bgGumps[2].Width, line3Y), 0, false, false);

            base.Draw(spriteBatch);
        }

        protected override void mouseClick(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Right)
            {
                if (CloseOnRightClick)
                    _owner.Dispose();
            }
        }
    }
}
