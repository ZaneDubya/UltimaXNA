/***************************************************************************
 *   ResizePic.cs
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
using UltimaXNA.Ultima.UI;

namespace UltimaXNA.Ultima.UI.Controls
{
    public class ResizePic : AControl
    {
        public bool CloseOnRightClick = false;
        Texture2D[] m_bgGumps = null;
        int GumpID = 0;

        public ResizePic(AControl owner, int page)
            : base(owner, page)
        {
            m_bgGumps = new Texture2D[9];
            HandlesMouseInput = true;
        }

        public ResizePic(AControl owner, int page, string[] arguements)
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

        public ResizePic(AControl owner, int page, int x, int y, int gumpID, int width, int height)
            : this(owner, page)
        {
            buildGumpling(x, y, gumpID, width, height);
        }

        public ResizePic(AControl owner, AControl c)
            : this(owner, c.Page)
        {
            buildGumpling(c.X - 4, c.Y - 4, 9350, c.Width + 8, c.Height + 8);
        }

        void buildGumpling(int x, int y, int gumpID, int width, int height)
        {
            MakeDragger(m_owner);
            Position = new Point(x, y);
            Size = new Point(width, height);
            GumpID = gumpID;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_bgGumps[0] == null)
            {
                for (int i = 0; i < 9; i++)
                {
                    m_bgGumps[i] = IO.GumpData.GetGumpXNA(GumpID + i);
                }
            }

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            int centerWidth = Width - m_bgGumps[0].Width - m_bgGumps[2].Width;
            int centerHeight = Height - m_bgGumps[0].Height - m_bgGumps[2].Height;
            int line2Y = Y + m_bgGumps[0].Height;
            int line3Y = Y + Height - m_bgGumps[6].Height;

            spriteBatch.Draw2D(m_bgGumps[0], new Point(X, Y), 0, false, false);
            spriteBatch.Draw2DTiled(m_bgGumps[1], new Rectangle(X + m_bgGumps[0].Width, Y, centerWidth, m_bgGumps[0].Height), 0, false, false);
            spriteBatch.Draw2D(m_bgGumps[2], new Point(X + Width - m_bgGumps[2].Width, Y), 0, false, false);

            spriteBatch.Draw2DTiled(m_bgGumps[3], new Rectangle(X, line2Y, m_bgGumps[0].Width, centerHeight), 0, false, false);
            spriteBatch.Draw2DTiled(m_bgGumps[4], new Rectangle(X + m_bgGumps[0].Width, line2Y, centerWidth, centerHeight), 0, false, false);
            spriteBatch.Draw2DTiled(m_bgGumps[5], new Rectangle(X + Width - m_bgGumps[2].Width, line2Y, m_bgGumps[2].Width, centerHeight), 0, false, false);

            spriteBatch.Draw2D(m_bgGumps[6], new Point(X, line3Y), 0, false, false);
            spriteBatch.Draw2DTiled(m_bgGumps[7], new Rectangle(X + m_bgGumps[0].Width, line3Y, centerWidth, m_bgGumps[6].Height), 0, false, false);
            spriteBatch.Draw2D(m_bgGumps[8], new Point(X + Width - m_bgGumps[2].Width, line3Y), 0, false, false);

            base.Draw(spriteBatch);
        }

        protected override void mouseClick(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Right)
            {
                if (CloseOnRightClick)
                    m_owner.Dispose();
            }
        }
    }
}
