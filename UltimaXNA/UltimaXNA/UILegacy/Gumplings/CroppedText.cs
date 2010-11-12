using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    class CroppedText : Control
    {
        public int Hue = 0;
        public string Text = string.Empty;

        public CroppedText(Control owner, int page)
            : base(owner, page)
        {

        }

        public CroppedText(Control owner, int page, string[] arguements, string[] lines)
            : this(owner, page)
        {
            int x, y, width, height, hue, textIndex;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            width = Int32.Parse(arguements[3]);
            height = Int32.Parse(arguements[4]);
            hue = Int32.Parse(arguements[5]);
            textIndex = Int32.Parse(arguements[6]);
            buildGumpling(x, y, width, height, hue, textIndex, lines);
        }

        public CroppedText(Control owner, int page, int x, int y, int width, int height, int hue, int textIndex, string[] lines)
            : this(owner, page)
        {
            buildGumpling(x, y, width, height, hue, textIndex, lines);
        }

        void buildGumpling(int x, int y, int width, int height, int hue, int textIndex, string[] lines)
        {
            Position = new Point2D(x, y);
            Size = new Point2D(width, height);
            Hue = hue;
            Text = lines[textIndex];
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            Texture2D texture = Data.UniText.GetTextureHTML(Text, Area.Width, Area.Height);
            spriteBatch.Draw2D(texture, Position, 0, false);
            base.Draw(spriteBatch);
        }
    }
}
