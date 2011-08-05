using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    class TextLabel : Control
    {
        public int Hue = 0;
        public string Text = string.Empty;
        TextRenderer _textRenderer = new TextRenderer();

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
            buildGumpling(x, y, hue, lines[textIndex]);
        }

        public TextLabel(Control owner, int page, int x, int y, int hue, string text)
            : this(owner, page)
        {
            buildGumpling(x, y, hue, text);
        }

        void buildGumpling(int x, int y, int hue, string text)
        {
            Position = new Point2D(x, y);
            Hue = hue;
            Text = text;
        }

        public override void Update(GameTime gameTime)
        {
            _textRenderer.RenderText(Text, true);
            if (_textRenderer.Texture == null)
            {
                _textRenderer.RenderText(Text, true);
                Size = new Point2D(_textRenderer.Texture.Width, _textRenderer.Texture.Height);
            }
            base.Update(gameTime);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            spriteBatch.Draw2D(_textRenderer.Texture, Position, Hue, false);
            base.Draw(spriteBatch);
        }
    }
}
