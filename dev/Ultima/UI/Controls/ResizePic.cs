/***************************************************************************
 *   ResizePic.cs
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;

namespace UltimaXNA.Ultima.UI.Controls {
    public class ResizePic : AControl {
        Texture2D[] m_Gumps;
        int GumpID;

        ResizePic(AControl parent)
            : base(parent) {
            m_Gumps = new Texture2D[9];
            MakeThisADragger();
        }

        public ResizePic(AControl parent, string[] arguements)
            : this(parent) {
            int x, y, gumpID, width, height;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            gumpID = Int32.Parse(arguements[3]);
            width = Int32.Parse(arguements[4]);
            height = Int32.Parse(arguements[5]);
            BuildGumpling(x, y, gumpID, width, height);
        }

        public ResizePic(AControl parent, int x, int y, int gumpID, int width, int height)
            : this(parent) {
            BuildGumpling(x, y, gumpID, width, height);
        }

        public ResizePic(AControl parent, AControl createBackgroundAroundThisControl)
            : this(parent) {
            BuildGumpling(createBackgroundAroundThisControl.X - 4,
                createBackgroundAroundThisControl.Y - 4,
                9350,
                createBackgroundAroundThisControl.Width + 8,
                createBackgroundAroundThisControl.Height + 8);
            Page = createBackgroundAroundThisControl.Page;
        }

        void BuildGumpling(int x, int y, int gumpID, int width, int height) {
            Position = new Point(x, y);
            Size = new Point(width, height);
            GumpID = gumpID;
        }

        public override void Update(double totalMS, double frameMS) {
            if (m_Gumps[0] == null) {
                IResourceProvider provider = Services.Get<IResourceProvider>();
                for (int i = 0; i < 9; i++) {
                    m_Gumps[i] = provider.GetUITexture(GumpID + i);
                }
            }
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS) {
            int centerWidth = Width - m_Gumps[0].Width - m_Gumps[2].Width;
            int centerHeight = Height - m_Gumps[0].Height - m_Gumps[6].Height;
            int line2Y = position.Y + m_Gumps[0].Height;
            int line3Y = position.Y + Height - m_Gumps[6].Height;
            // top row
            spriteBatch.Draw2D(m_Gumps[0], new Vector3(position.X, position.Y, 0), Vector3.Zero);
            spriteBatch.Draw2DTiled(m_Gumps[1], new Rectangle(position.X + m_Gumps[0].Width, position.Y, centerWidth, m_Gumps[0].Height), Vector3.Zero);
            spriteBatch.Draw2D(m_Gumps[2], new Vector3(position.X + Width - m_Gumps[2].Width, position.Y, 0), Vector3.Zero);
            // middle
            spriteBatch.Draw2DTiled(m_Gumps[3], new Rectangle(position.X, line2Y, m_Gumps[3].Width, centerHeight), Vector3.Zero);
            spriteBatch.Draw2DTiled(m_Gumps[4], new Rectangle(position.X + m_Gumps[3].Width, line2Y, centerWidth, centerHeight), Vector3.Zero);
            spriteBatch.Draw2DTiled(m_Gumps[5], new Rectangle(position.X + Width - m_Gumps[5].Width, line2Y, m_Gumps[5].Width, centerHeight), Vector3.Zero);
            // bottom
            spriteBatch.Draw2D(m_Gumps[6], new Vector3(position.X, line3Y, 0), Vector3.Zero);
            spriteBatch.Draw2DTiled(m_Gumps[7], new Rectangle(position.X + m_Gumps[6].Width, line3Y, centerWidth, m_Gumps[6].Height), Vector3.Zero);
            spriteBatch.Draw2D(m_Gumps[8], new Vector3(position.X + Width - m_Gumps[8].Width, line3Y, 0), Vector3.Zero);

            base.Draw(spriteBatch, position, frameMS);
        }
    }
}