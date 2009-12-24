using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

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
            HandlesMouseInput = true;
            HandlesKeyboardFocus = true;
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
            _isBlinkOn = false;
        }

        bool _isBlinkOn = false;
        float _secondsSinceLastBlink = 0f;
        const float _SecondsPerBlink = 0.5f;
        public override void Update(GameTime gameTime)
        {
            if (_textChanged)
            {
                _textChanged = false;
                _texture = Data.UniText.GetTextTexture(Text, 1, false);
            }

            if (_manager.KeyboardFocusControl == this)
            {
                _secondsSinceLastBlink += ((float)gameTime.ElapsedRealTime.Milliseconds / 1000f);
                if (_secondsSinceLastBlink >= _SecondsPerBlink)
                {
                    _secondsSinceLastBlink -= _SecondsPerBlink;
                    if (_isBlinkOn == true)
                        _isBlinkOn = false;
                    else
                        _isBlinkOn = true;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(UltimaXNA.Graphics.ExtendedSpriteBatch spriteBatch)
        {
            if (_isBlinkOn)
            {
                spriteBatch.Draw(_texture, new Vector2(Area.X, Area.Y), HueColor(Hue));
                Texture2D caratTexture = Data.UniText.GetTextTexture("|", 1, false);
                spriteBatch.Draw(caratTexture, new Vector2(Area.X + _texture.Width, Area.Y), HueColor(Hue));
            }
            else
            {
                spriteBatch.Draw(_texture, new Vector2(Area.X, Area.Y), HueColor(Hue));
            }
            
            base.Draw(spriteBatch);
        }

        protected override void _keyboardInput(string keys, List<Keys> specials)
        {
            Text += keys;
            foreach (Keys key in specials)
            {
                switch (key)
                {
                    case Keys.Back:
                        if (Text.Length > 0)
                        {
                            Text = Text.Substring(0, Text.Length - 1);
                        }
                        break;
                }
            }
        }
    }
}
