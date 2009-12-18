using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    class TextLabel : Control
    {
        public int Hue = 0;
        public string Text = string.Empty;
        Texture2D _texture = null;

        public TextLabel(Control owner, int page)
            : base(owner, page)
        {

        }

        public TextLabel(Control owner, int page, string[] arguements, string[] lines)
            : this(owner, page)
        {
            int x, y, hue, textIndex;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            hue = Int32.Parse(arguements[3]);
            textIndex = Int32.Parse(arguements[4]);
            buildGumpling(x, y, hue, textIndex, lines);
        }

        public TextLabel(Control owner, int page, int x, int y, int hue, int textIndex, string[] lines)
            : this(owner, page)
        {
            buildGumpling(x, y, hue, textIndex, lines);
        }

        void buildGumpling(int x, int y, int hue, int textIndex, string[] lines)
        {
            Position = new Vector2(x, y);
            Hue = hue;
            Text = lines[textIndex];
            _texture = Data.UniText.GetTextTexture(Text, 1, false);
            Size = new Vector2(_texture.Width, _texture.Height);
        }

        public override void Draw(UltimaXNA.Graphics.ExtendedSpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, new Vector2(Area.X, Area.Y), HueColor(Hue));
            base.Draw(spriteBatch);
        }
    }
}
