using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    class HtmlGump : Control
    {
        Texture2D _renderedHtml = null;
        public string Text = string.Empty;
        bool _background = false;
        bool _scrollbar = false;

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

        public override void Draw(UltimaXNA.Graphics.ExtendedSpriteBatch spriteBatch)
        {
            if (_renderedHtml == null)
            {
                Color[] data = new Color[Width * Height];
                for (int h = 0; h < Height; h++)
                {
                    for (int w = 0; w < Width; w++)
                    {
                        data[h * Width + w] = new Color(0f, 0f, 0f, 0f);
                    }
                }
                _renderedHtml = new Texture2D(spriteBatch.GraphicsDevice, Width, Height);
                _renderedHtml.SetData<Color>(data);
            }

            spriteBatch.Draw(_renderedHtml, new Rectangle(Area.X, Area.Y, Area.Width, Area.Height), new Rectangle(0, 0, Area.Width, Area.Height), Color.White);

            Texture2D texture = Data.UniText.GetTextTexture(Text, 1, true);
            spriteBatch.Draw(texture, new Vector2(Area.X, Area.Y), Color.White);

            base.Draw(spriteBatch);
        }
    }
}
