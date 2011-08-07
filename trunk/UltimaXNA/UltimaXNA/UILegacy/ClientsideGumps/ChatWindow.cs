using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.UILegacy.Gumplings;

namespace UltimaXNA.UILegacy.ClientsideGumps
{
    class ChatWindow : Gump
    {
        TextEntry _input;
        List<ChatLineTimed> _textEntries;

        public ChatWindow()
            : base(0, 0)
        {
            _textEntries = new List<ChatLineTimed>();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (_input == null)
            {
                _input = new TextEntry(this, 0, 1, _manager.Height - Data.UniText.FontHeight(0) + 4, 400, Data.UniText.FontHeight(0), 0, 0, 64, string.Empty);
                _input.LegacyCarat = true;
                AddGumpling(_input);
            }

            int y = _input.Y - 48;
            for (int i = 0; i < _textEntries.Count; i++)
            {
                _textEntries[i].Update(gameTime);
                if (_textEntries[i].IsExpired)
                {
                    _textEntries[i].Dispose();
                    _textEntries.RemoveAt(i);
                    i--;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            int y = _input.Y - 48;
            for (int i = _textEntries.Count - 1; i >= 0; i--)
            {
                spriteBatch.Draw2D(_textEntries[i].Texture, new Point2D(1, y), 0, true);
                y -= _textEntries[i].Texture.Height;
            }
            base.Draw(spriteBatch);
        }

        public override void ActivateByKeyboardReturn(int textID, string text)
        {
            _input.Text = string.Empty;
            Interaction.SendChat(text);
        }

        public void AddLine(string text)
        {
            _textEntries.Add(new ChatLineTimed(text, Width));
        }
    }

    class ChatLineTimed
    {
        string _text;
        public string Text { get { return _text; } }
        float _createdTime = float.MinValue;
        bool _isExpired;
        public bool IsExpired { get { return _isExpired; } }
        float _alpha;
        public float Alpha { get { return _alpha; } }

        const float Time_Display = 10.0f;
        const float Time_Fadeout = 5.0f;

        private TextRenderer _renderer;
        public Texture2D Texture { get { return _renderer.Texture; } }

        public ChatLineTimed(string text, int width)
        {
            _text = text;
            _isExpired = false;
            _alpha = 1.0f;

            _renderer = new TextRenderer();
            _renderer.RenderText(_text, false, width, 0);
        }

        public void Update(GameTime gameTime)
        {
            if (_createdTime == float.MinValue)
                _createdTime = (float)gameTime.TotalGameTime.TotalSeconds;
            float time = (float)gameTime.TotalGameTime.TotalSeconds - _createdTime;
            if (time > Time_Display + Time_Fadeout)
                _isExpired = true;
            else if (time > Time_Display)
            {
                _alpha = (time - Time_Display) / Time_Fadeout;
            }
        }

        public void Dispose()
        {
            _renderer = null;
        }
    }
}
