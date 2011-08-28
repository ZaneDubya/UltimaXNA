/***************************************************************************
 *   ChatHandler.cs
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

namespace UltimaXNA.UILegacy
{
    class ChatHandler
    {
        List<ChatLine> _textLines = new List<ChatLine>();
        List<GameTime> _textTimes = new List<GameTime>();

        public void Clear()
        {
            _textLines = new List<ChatLine>();
            _textTimes = new List<GameTime>();
        }

        public void Update(GameTime gameTime, bool eraseOldEntries)
        {
            List<int> indexesToRemove = new List<int>();

            for (int i = 0; i < _textLines.Count; i++)
            {
                if (_textTimes[i].TotalGameTime.Ticks == 0)
                {
                    _textTimes[i] = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime);
                }

                if (eraseOldEntries && gameTime.TotalGameTime.TotalSeconds - _textTimes[i].TotalGameTime.TotalSeconds >= 10.0f)
                {
                    indexesToRemove.Add(i);
                }
            }

            for (int i = 0; i < indexesToRemove.Count; i++)
            {
                _textLines.RemoveAt(indexesToRemove[i] - i);
                _textTimes.RemoveAt(indexesToRemove[i] - i);
            }
        }

        public List<ChatLine> GetMessages()
        {
            return this.GetMessages(-1);
        }

        public List<ChatLine> GetMessages(int count)
        {
            if (count == -1)
                count = _textLines.Count;

            List<ChatLine> list = new List<ChatLine>();
            for (int i = 0; i < count && i < _textLines.Count; i++)
            {
                list.Add(_textLines[i]);
            }
            return list;
        }

        public void AddMessage(string text)
        {
            _textLines.Add(new ChatLine(text));
            _textTimes.Add(new GameTime());
        }

        public void AddMessage(string text, int hue, int font)
        {
            _textLines.Add(new ChatLine(text, hue, font));
            _textTimes.Add(new GameTime());
        }
    }

    class ChatLine
    {
        public string Text;
        public int Hue;
        public int Font;

        public ChatLine(string text, int hue, int font)
        {
            Text = text;
            Hue = hue;
            Font = font;
        }

        public ChatLine(string text)
            : this(text, 0, 0)
        {
        }
    }
}
