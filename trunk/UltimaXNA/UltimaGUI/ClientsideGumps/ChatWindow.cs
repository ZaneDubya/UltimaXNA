/***************************************************************************
 *   ChatWindow.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.Interface.Graphics;
using UltimaXNA.UltimaGUI.Gumplings;

namespace UltimaXNA.UltimaGUI.ClientsideGumps
{
    class ChatWindow : Gump
    {
        TextEntry InputState;
        List<ChatLineTimed> _textEntries;

        public ChatWindow()
            : base(0, 0)
        {
            _textEntries = new List<ChatLineTimed>();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (InputState == null)
            {
                InputState = new TextEntry(this, 0, 1, UserInterface.Height - UltimaData.UniText.FontHeight(0) + 4, 400, UltimaData.UniText.FontHeight(0), 0, 0, 64, string.Empty);
                InputState.LegacyCarat = true;
                AddControl(InputState);
            }

            int y = InputState.Y - 48;
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

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            int y = InputState.Y - 20;
            for (int i = _textEntries.Count - 1; i >= 0; i--)
            {
                y -= _textEntries[i].TextHeight;
                _textEntries[i].Draw(spriteBatch, new Point2D(1, y));
            }
            base.Draw(spriteBatch);
        }

        public override void ActivateByKeyboardReturn(int textID, string text)
        {
            InputState.Text = string.Empty;
            UltimaInteraction.SendChat(text);
        }

        public void AddLine(string text)
        {
            _textEntries.Add(new ChatLineTimed(string.Format("<{1}>{0}</{1}>", text, "big"), Width));
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
        private int _width = 0;

        const float Time_Display = 10.0f;
        const float Time_Fadeout = 4.0f;

        private Interface.GUI.TextRenderer _renderer;
        public int TextHeight { get { return _renderer.Height; } }

        public ChatLineTimed(string text, int width)
        {
            _text = text;
            _isExpired = false;
            _alpha = 1.0f;
            _width = width;

            _renderer = new Interface.GUI.TextRenderer(_text, _width, true);
        }

        public void Update(GameTime gameTime)
        {
            if (_createdTime == float.MinValue)
                _createdTime = (float)gameTime.TotalGameTime.TotalSeconds;
            float time = (float)gameTime.TotalGameTime.TotalSeconds - _createdTime;
            if (time > Time_Display)
                _isExpired = true;
            else if (time > (Time_Display - Time_Fadeout))
            {
                _alpha = 1.0f - ((time) - (Time_Display - Time_Fadeout)) / Time_Fadeout;
            }
            _renderer.Transparent = (_alpha < 1.0f);
        }

        public void Draw(SpriteBatchUI sb, Point2D position)
        {
            _renderer.Draw(sb, position);
        }

        public void Dispose()
        {
            _renderer = null;
        }

        public override string ToString()
        {
            return _text;
        }
    }
}
