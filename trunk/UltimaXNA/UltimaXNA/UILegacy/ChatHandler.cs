using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                if (_textTimes[i].TotalRealTime.Ticks == 0)
                {
                    _textTimes[i] = new GameTime(gameTime.TotalRealTime, gameTime.ElapsedRealTime, gameTime.TotalGameTime, gameTime.ElapsedGameTime);
                }

                if (eraseOldEntries && gameTime.TotalRealTime.TotalSeconds - _textTimes[i].TotalRealTime.TotalSeconds >= 10.0f)
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
