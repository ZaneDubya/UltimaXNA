using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Graphics;

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
            Position = new Point2D(x, y);
            Size = new Point2D(width, height);
            Hue = hue;
            FontID = fontid;
            Text = text;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            if (_texture == null)
                _texture = Data.ASCIIText.GetTextTexture(Text, FontID, Area.Width);
            spriteBatch.Draw2D(_texture, Position, Hue, true, false);
            base.Draw(spriteBatch);
        }
    }
}
