/***************************************************************************
 *   ChatWindow.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Rendering;
using UltimaXNA.UltimaGUI.Controls;
using UltimaXNA.UltimaData.FontsNew;
using InterXLib.Input.Windows;

namespace UltimaXNA.UltimaGUI.WorldGumps
{
    class ChatWindow : Gump
    {
        TextEntry InputState;
        List<ChatLineTimed> m_textEntries;
        List<string> m_messages;
        int messageIndex = -1;

        public ChatWindow()
            : base(0, 0)
        {
            m_textEntries = new List<ChatLineTimed>();
            m_messages = new List<string>();
            Width = 400;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (InputState == null)
            {
                InputState = new TextEntry(this, 0, 1, UserInterface.Height - TextUni.GetFont(0).Height + 4, 400, TextUni.GetFont(0).Height, 0, 0, 64, string.Empty);
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

            if (UltimaEngine.Input.HandleKeyboardEvent(KeyboardEventType.Down, WinKeys.Q, false, false, true) && messageIndex > -1)
            {
                InputState.Text = m_messages[messageIndex];
                if (messageIndex > 0)
                    messageIndex -= 1;
            }
            //Zane must give attention, i couldn't deny keyboard taking q as an input, it works but add q key at the end of the message
            /*else if (UltimaEngine.Input.HandleKeyboardEvent(KeyboardEventType.Down, WinKeys.Q, false, true, false) && messageIndex < m_messages.Count-1)
            {
                messageIndex += 1;
                InputState.Text = m_messages[messageIndex];
            }
            */
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
            m_messages.Add(text);
            messageIndex = m_messages.Count-1;
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

        private RenderedText m_Texture;
        public int TextHeight { get { return m_Texture.Height; } }

        public ChatLineTimed(string text, int width)
        {
            m_text = text;
            m_isExpired = false;
            m_alpha = 1.0f;
            m_width = width;

            m_Texture = new RenderedText(m_text, true, m_width);
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
            m_Texture.Transparent = (m_alpha < 1.0f);
        }

        public void Draw(SpriteBatchUI sb, Point position)
        {
            m_Texture.Draw(sb, position);
        }

        public void Dispose()
        {
            m_Texture = null;
        }

        public override string ToString()
        {
            return m_text;
        }
    }
}
