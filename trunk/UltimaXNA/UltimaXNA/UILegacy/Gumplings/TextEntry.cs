using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.Input;
using UltimaXNA.Graphics;

namespace UltimaXNA.UILegacy.Gumplings
{
    class TextEntry : Control
    {
        public int Hue = 0;
        public int EntryID = 0;
        public int LimitSize = 0;
        public bool IsPasswordField = false;

        string _text = string.Empty;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
            }
        }

        public string HtmlTag = string.Empty;

        bool _legacyCarat = false;
        public bool LegacyCarat { get { return _legacyCarat; } set { _legacyCarat = value; } }

        bool _isFocused = false;
        bool _caratBlinkOn = false;
        float _secondsSinceLastBlink = 0f;
        const float _SecondsPerBlink = 0.5f;

        TextRenderer _textRenderer;
        TextRenderer _caratRenderer;

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
            Position = new Point2D(x, y);
            Size = new Point2D(width, height);
            Hue = hue;
            EntryID = entryID;
            Text = text;
            LimitSize = limitSize;
            _caratBlinkOn = false;
            _textRenderer = new TextRenderer("", width, true);
            _caratRenderer = new TextRenderer("", width, true);
        }

        public override void Update(GameTime gameTime)
        {
            if (_manager.KeyboardFocusControl == this)
            {
                // if we're not already focused, turn the carat on immediately.
                // if we're using the legacy carat, keep it visible. Else blink it every x seconds.
                if (!_isFocused)
                {
                    _isFocused = true;
                    _caratBlinkOn = true;
                    _secondsSinceLastBlink = 0f;
                }
                if (_legacyCarat)
                    _caratBlinkOn = true;
                else
                {
                    _secondsSinceLastBlink += ((float)gameTime.ElapsedGameTime.TotalSeconds);
                    if (_secondsSinceLastBlink >= _SecondsPerBlink)
                    {
                        _secondsSinceLastBlink -= _SecondsPerBlink;
                        if (_caratBlinkOn == true)
                            _caratBlinkOn = false;
                        else
                            _caratBlinkOn = true;
                    }
                }
            }
            else
            {
                _isFocused = false;
                _caratBlinkOn = false;
            }

            _textRenderer.Text = HtmlTag + (IsPasswordField ? new string('*', Text.Length) : Text);
            _caratRenderer.Text = HtmlTag + (_legacyCarat ? "_" : "|");

            base.Update(gameTime);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            _textRenderer.Draw(spriteBatch, Position);
            if (_caratBlinkOn)
                _caratRenderer.Draw(spriteBatch, new Point2D(X + _textRenderer.TextureWidth, Y));
            
            base.Draw(spriteBatch);
        }

        protected override void keyboardInput(InputEventKB e)
        {
            switch (e.KeyCode)
            {
                case WinKeys.Back:
                    if (Text.Length > 0)
                    {
                        Text = Text.Substring(0, Text.Length - 1);
                    }
                    break;
                case WinKeys.Tab:
                    _owner.ReleaseKeyboardInput(this);
                    break;
                case WinKeys.Enter:
                    _owner.ActivateByKeyboardReturn(EntryID, Text);
                    break;
                default:
                    if (e.IsChar)
                    {
                        Text += e.KeyChar;
                    }
                    break;
            }
        }
    }
}
