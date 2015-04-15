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
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.UI;

namespace UltimaXNA.Ultima.UI.Controls
{
    class CheckerTrans : AControl
    {
        private static Texture2D s_CheckeredTransTexture = null;
        public static Texture2D CheckeredTransTexture
        {
            get
            {
                if (s_CheckeredTransTexture == null)
                {
                    Color[] data = new Color[32 * 32];
                    for (int h = 0; h < 32; h++)
                    {
                        int i = h % 2;
                        for (int w = 0; w < 32; w++)
                        {
                            if (i++ >= 1)
                            {
                                data[h * 32 + w] = Color.Black;
                                i = 0;
                            }
                            else
                            {
                                data[h * 32 + w] = Color.Transparent;
                            }
                        }
                    }
                    SpriteBatchUI sb = UltimaServices.GetService<SpriteBatchUI>();
                    s_CheckeredTransTexture = new Texture2D(sb.GraphicsDevice, 32, 32);
                    s_CheckeredTransTexture.SetData<Color>(data);
                }
                return s_CheckeredTransTexture;
            }
        }

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
            // spriteBatch.Flush();
            // spriteBatch.Begin(SpriteBlendMode.None); !!!
            spriteBatch.Draw2DTiled(CheckeredTransTexture, new Rectangle(X, Y, Width, Area.Height), 0, false, false);
            // spriteBatch.Flush();

            base.Draw(spriteBatch);
        }
    }
}
