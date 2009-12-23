using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    class CheckerTrans : Control
    {
        Texture2D _renderedTexture = null;

        public CheckerTrans(Control owner, int page)
            : base(owner, page)
        {

        }

        public CheckerTrans(Control owner, int page, string[] arguements)
            : this(owner, page)
        {
            int x, y, width, height;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            width = Int32.Parse(arguements[3]);
            height = Int32.Parse(arguements[4]);

            buildGumpling(x, y, width, height);
        }

        public CheckerTrans(Control owner, int page, int x, int y, int width, int height)
            : this(owner, page)
        {
            buildGumpling(x, y, width, height);
        }

        void buildGumpling(int x, int y, int width, int height)
        {
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(UltimaXNA.Graphics.ExtendedSpriteBatch spriteBatch)
        {
            if (_renderedTexture == null)
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
                            data[h * Width + w] = Color.TransparentBlack;
                        }
                    }
                }
                _renderedTexture = new Texture2D(spriteBatch.GraphicsDevice, Width, Height);
                _renderedTexture.SetData<Color>(data);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteBlendMode.None);
            spriteBatch.Draw(_renderedTexture, new Rectangle(Area.X, Area.Y, Area.Width, Area.Height), new Rectangle(0, 0, Area.Width, Area.Height), Color.White);
            spriteBatch.End();
            spriteBatch.Begin();

            base.Draw(spriteBatch);
        }
    }
}
