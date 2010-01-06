using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    class HtmlGump : Control
    {
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

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            Texture2D texture = Data.UniText.GetTexture(Text, Area.Width, Area.Height);
            spriteBatch.Draw(texture, new Vector2(Area.X, Area.Y), Color.White);

            base.Draw(spriteBatch);
        }
    }
}
