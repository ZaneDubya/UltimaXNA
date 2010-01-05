using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    class TextLabelAsciiCropped : Control
    {
        public int Hue = 0;
        public int FontID = 0;
        public string Text = string.Empty;
        Texture2D _texture = null;

        public TextLabelAsciiCropped(Control owner, int page)
            : base(owner, page)
        {

        }

        public TextLabelAsciiCropped(Control owner, int page, int x, int y, int width, int height, int hue, int fontid, string text)
            : this(owner, page)
        {
            buildGumpling(x, y, width, height, hue, fontid, text);
        }

        void buildGumpling(int x, int y, int width, int height, int hue, int fontid, string text)
        {
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
            Hue = hue;
            FontID = fontid;
            Text = text;
        }

        public override void Update(GameTime gameTime)
        {
            if (_texture == null)
            {
                _texture = Data.ASCIIText.GetTextTexture(Text, FontID, Area.Width);
                // Size = new Vector2(_texture.Width, _texture.Height);
            }
            base.Update(gameTime);
        }

        public override void Draw(UltimaXNA.Graphics.ExtendedSpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, new Vector2(Area.X, Area.Y), HueColor(Hue, true));
            base.Draw(spriteBatch);
        }
    }
}
