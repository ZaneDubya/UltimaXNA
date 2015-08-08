/***************************************************************************
 *   ChatControl.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Resources.Fonts;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class ChatControl : AControl
    {
        private const int MaxChatMessageLength = 96;

        TextEntry m_TextEntry;
        List<ChatLineTimed> m_TextEntries;
        List<string> m_MessageHistory;

        UserInterfaceService m_UserInterface;
        InputManager m_Input;
        WorldModel m_World;

        int m_MessageHistoryIndex = -1;

        public ChatControl(AControl parent, int x, int y, int width, int height)
            : base(parent)
        {
            Position = new Point(x, y);
            Size = new Point(width, height);

            m_TextEntries = new List<ChatLineTimed>();
            m_MessageHistory = new List<string>();

            m_Input = ServiceRegistry.GetService<InputManager>();
            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
            m_World = ServiceRegistry.GetService<WorldModel>();

            IsUncloseableWithRMB = true;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_TextEntry == null)
            {
                IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
                IFont font = provider.GetUnicodeFont(0);
                m_TextEntry = new TextEntry(this, 1, Height - font.Height, Width, font.Height, 0, 0, MaxChatMessageLength, string.Empty);
                m_TextEntry.LegacyCarat = true;

                AddControl(new CheckerTrans(this, 0, Height - 20, Width, 20));
                AddControl(m_TextEntry);
            }

            for (int i = 0; i < m_TextEntries.Count; i++)
            {
                m_TextEntries[i].Update(totalMS, frameMS);
                if (m_TextEntries[i].IsExpired)
                {
                    m_TextEntries[i].Dispose();
                    m_TextEntries.RemoveAt(i);
                    i--;
                }
            }

            // Ctrl-Q = Cycle backwards through the things you have said today
            // Ctrl-W = Cycle forwards through the things you have said today
            if (m_Input.HandleKeyboardEvent(KeyboardEventType.Down, WinKeys.Q, false, false, true) && m_MessageHistoryIndex > -1)
            {
                if (m_MessageHistoryIndex > 0)
                    m_MessageHistoryIndex -= 1;
                m_TextEntry.Text = m_MessageHistory[m_MessageHistoryIndex];

            }
            else if (m_Input.HandleKeyboardEvent(KeyboardEventType.Down, WinKeys.W, false, false, true))
            {
                if (m_MessageHistoryIndex < m_MessageHistory.Count - 1)
                {
                    m_MessageHistoryIndex += 1;
                    m_TextEntry.Text = m_MessageHistory[m_MessageHistoryIndex];
                }
                else
                    m_TextEntry.Text = string.Empty;
            }

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            int y = m_TextEntry.Y + position.Y - 6;
            for (int i = m_TextEntries.Count - 1; i >= 0; i--)
            {
                y -= m_TextEntries[i].TextHeight;
                m_TextEntries[i].Draw(spriteBatch, new Point(position.X + 2, y));
            }
            base.Draw(spriteBatch, position);
        }

        public override void ActivateByKeyboardReturn(int textID, string text)
        {
            m_TextEntry.Text = string.Empty;
            m_MessageHistory.Add(text);
            m_MessageHistoryIndex = m_MessageHistory.Count;
            m_World.Interaction.SendChat(text);
        }

        public void AddLine(string text)
        {
            m_TextEntries.Add(new ChatLineTimed(string.Format("<{1}>{0}</{1}>", text, "big"), Width));
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

        const float Time_Display = 10000.0f;
        const float Time_Fadeout = 4000.0f;

        private RenderedText m_Texture;
        public int TextHeight { get { return m_Texture.Height; } }

        public ChatLineTimed(string text, int width)
        {
            m_text = text;
            m_isExpired = false;
            m_alpha = 1.0f;
            m_width = width;

            m_Texture = new RenderedText(m_text, m_width);
        }

        public void Update(double totalMS, double frameMS)
        {
            if (m_createdTime == float.MinValue)
                m_createdTime = (float)totalMS;
            float time = (float)totalMS - m_createdTime;
            if (time > Time_Display)
                m_isExpired = true;
            else if (time > (Time_Display - Time_Fadeout))
            {
                m_alpha = 1.0f - ((time) - (Time_Display - Time_Fadeout)) / Time_Fadeout;
            }
        }

        public void Draw(SpriteBatchUI sb, Point position)
        {
            m_Texture.Draw(sb, position, Utility.GetHueVector(0, false, (m_alpha < 1.0f)));
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
