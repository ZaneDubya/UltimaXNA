using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    class TextEntry : Control
    {
        public int Hue = 0;
        public int EntryID = 0;
        public int LimitSize = 0;
        Texture2D _texture = null;

        bool _textChanged = false;
        string _text = string.Empty;
        public string Text
        {
            get { return _text; }
            set
            {
                _textChanged = true;
                _text = value;
            }
        }

        public TextEntry(Control owner, int page)
            : base(owner, page)
        {

        }

        public TextEntry(Control owner, int page, string[] arguements, string[] lines)
            : this(owner, page)
        {
            int x, y, width, height, hue, entryID, textIndex, limitSize = 0;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            width = Int32.Parse(arguements[3]);
            height = Int32.Parse(arguements[4]);
            hue = Int32.Parse(arguements[5]);
            entryID = Int32.Parse(arguements[6]);
            textIndex = Int32.Parse(arguements[7]);
            if (arguements[0] == "textentrylimited")
            {
                limitSize = Int32.Parse(arguements[8]);
            }
            buildGumpling(x, y, width, height, hue, entryID, limitSize, lines[textIndex]);
        }

        public TextEntry(Control owner, int page, int x, int y, int width, int height, int hue, int entryID, int limitSize, string text)
            : this(owner, page)
        {
            buildGumpling(x, y, width, height, hue, entryID, limitSize, text);
        }

        void buildGumpling(int x, int y, int width, int height, int hue, int entryID, int limitSize, string text)
        {
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
            Hue = hue;
            EntryID = entryID;
            Text = text;
            LimitSize = limitSize;
            Size = new Vector2(width, height);
        }

        public override void Update(GameTime gameTime)
        {
            if (_textChanged)
            {
                _textChanged = false;
                _texture = Data.UniText.GetTextTexture(Text, 1, false);
            }
            base.Update(gameTime);
        }

        public override void Draw(UltimaXNA.Graphics.ExtendedSpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, new Vector2(Area.X, Area.Y), HueColor(Hue));
            base.Draw(spriteBatch);
        }
    }
}
