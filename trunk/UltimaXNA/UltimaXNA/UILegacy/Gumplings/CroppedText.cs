using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    class CroppedText : Control
    {
        public int Hue = 0;
        public string Text = string.Empty;

        public CroppedText(Serial serial, Control owner)
            : base(serial, owner)
        {

        }

        public CroppedText(Serial serial, Control owner, string[] arguements, string[] lines)
            : this(serial, owner)
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

        public CroppedText(Serial serial, Control owner, int x, int y, int width, int height, int hue, int textIndex, string[] lines)
            : this(serial, owner)
        {
            buildGumpling(x, y, width, height, hue, textIndex, lines);
        }

        void buildGumpling(int x, int y, int width, int height, int hue, int textIndex, string[] lines)
        {
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
            Hue = hue;
            Text = lines[textIndex];
        }

        public override void Draw(UltimaXNA.Graphics.ExtendedSpriteBatch spriteBatch)
        {
            Texture2D texture = Data.ASCIIText.GetTextTexture(Text, 1);
            spriteBatch.Draw(texture, new Vector2(Area.X, Area.Y), Color.White);
            base.Draw(spriteBatch);
        }
    }
}
