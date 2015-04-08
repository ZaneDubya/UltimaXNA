/***************************************************************************
 *   CheckerTrans.cs
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
using UltimaXNA.Core.Rendering;
using UltimaXNA.UltimaGUI;

namespace UltimaXNA.UltimaGUI.Controls
{
    class CheckerTrans : AControl
    {
        Texture2D m_renderedTexture = null;

        public CheckerTrans(AControl owner, int page)
            : base(owner, page)
        {

        }

        public CheckerTrans(AControl owner, int page, string[] arguements)
            : this(owner, page)
        {
            int x, y, width, height;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            width = Int32.Parse(arguements[3]);
            height = Int32.Parse(arguements[4]);

            buildGumpling(x, y, width, height);
        }

        public CheckerTrans(AControl owner, int page, int x, int y, int width, int height)
            : this(owner, page)
        {
            buildGumpling(x, y, width, height);
        }

        void buildGumpling(int x, int y, int width, int height)
        {
            Position = new Point(x, y);
            Size = new Point(width, height);
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            if (m_renderedTexture == null)
            {
                Color[] data = new Color[Width * Height];
                for (int h = 0; h < Height; h++)
                {
                    int i = h % 2;
                    for (int w = 0; w < Width; w++)
                    {
                        if (i++ >= 1)
                        {
                            data[h * Width + w] = Color.Black;
                            i = 0;
                        }
                        else
                        {
                            data[h * Width + w] = Color.Transparent;
                        }
                    }
                }
                m_renderedTexture = new Texture2D(spriteBatch.GraphicsDevice, Width, Height);
                m_renderedTexture.SetData<Color>(data);
            }

            // spriteBatch.Flush();
            // spriteBatch.Begin(SpriteBlendMode.None); !!!
            spriteBatch.Draw2D(m_renderedTexture, new Rectangle(X, Y, Width, Area.Height), new Rectangle(0, 0, Area.Width, Area.Height), 0, false, false);
            // spriteBatch.Flush();

            base.Draw(spriteBatch);
        }
    }
}
