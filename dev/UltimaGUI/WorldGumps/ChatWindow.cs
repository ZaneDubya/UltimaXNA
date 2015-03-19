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
using UltimaXNA.Rendering;
using UltimaXNA.UltimaGUI.Controls;
using UltimaXNA.UltimaData.Fonts;

namespace UltimaXNA.UltimaGUI.WorldGumps
{
    class ChatWindow : Gump
    {
        TextEntry InputState;
        List<ChatLineTimed> m_textEntries;

        public ChatWindow()
            : base(0, 0)
        {
            m_textEntries = new List<ChatLineTimed>();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (InputState == null)
            {
                InputState = new TextEntry(this, 0, 1, UserInterface.Height - UniText.FontHeight(0) + 4, 400, UniText.FontHeight(0), 0, 0, 64, string.Empty);
                InputState.LegacyCarat = true;
                AddControl(InputState);
            }

            int y = InputState.Y - 48;
            for (int i = 0; i < m_textEntries.Count; i++)
            {
                m_textEntries[i].Update(gameTime);
                if (m_textEntries[i].IsExpired)
                {
                    m_textEntries[i].Dispose();
                    m_textEntries.RemoveAt(i);
                    i--;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            int y = InputState.Y - 20;
            for (int i = m_textEntries.Count - 1; i >= 0; i--)
            {
                y -= m_textEntries[i].TextHeight;
                m_textEntries[i].Draw(spriteBatch, new Point(1, y));
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
            m_textEntries.Add(new ChatLineTimed(string.Format("<{1}>{0}</{1}>", text, "big"), Width));
        }
    }

    class ChatLineTimed
    {
        string m_text;
        public string Text { get { return m_text; } }
        float m_createdTime = float.MinValue;
        bool m_isExpired;
        public bool IsExpired { get { return m_isExpired; } }
        float m_alpha;
        public float Alpha { get { return m_alpha; } }
        private int m_width = 0;

        const float Time_Display = 10.0f;
        const float Time_Fadeout = 4.0f;

        private TextRenderer m_renderer;
        public int TextHeight { get { return m_renderer.Height; } }

        public ChatLineTimed(string text, int width)
        {
            m_text = text;
            m_isExpired = false;
            m_alpha = 1.0f;
            m_width = width;

            m_renderer = new TextRenderer(m_text, m_width, true);
        }

        public void Update(GameTime gameTime)
        {
            if (m_createdTime == float.MinValue)
                m_createdTime = (float)gameTime.TotalGameTime.TotalSeconds;
            float time = (float)gameTime.TotalGameTime.TotalSeconds - m_createdTime;
            if (time > Time_Display)
                m_isExpired = true;
            else if (time > (Time_Display - Time_Fadeout))
            {
                m_alpha = 1.0f - ((time) - (Time_Display - Time_Fadeout)) / Time_Fadeout;
            }
            m_renderer.Transparent = (m_alpha < 1.0f);
        }

        public void Draw(SpriteBatchUI sb, Point position)
        {
            m_renderer.Draw(sb, position);
        }

        public void Dispose()
        {
            m_renderer = null;
        }

        public override string ToString()
        {
            return m_text;
        }
    }
}
