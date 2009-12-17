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

        public HtmlGump(Serial serial, Control owner)
            : base(serial, owner)
        {

        }

        public HtmlGump(Serial serial, Control owner, string[] arguements, string[] lines)
            : this(serial, owner)
        {
            int x, y, width, height, textIndex, background, scrollbar;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            width = Int32.Parse(arguements[3]);
            height = Int32.Parse(arguements[4]);
            textIndex = Int32.Parse(arguements[5]);
            background = Int32.Parse(arguements[6]);
            scrollbar = Int32.Parse(arguements[7]);

            buildGumpling(x, y, width, height, textIndex, background, scrollbar, lines);
        }

        public HtmlGump(Serial serial, Control owner, int x, int y, int width, int height, int textIndex, int background, int scrollbar, string[] lines)
            : this(serial, owner)
        {
            buildGumpling(x, y, width, height, textIndex, background, scrollbar, lines);
        }

        void buildGumpling(int x, int y, int width, int height, int textIndex, int background, int scrollbar, string[] lines)
        {
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
            Text = lines[textIndex];
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
