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
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.Windows;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.Data;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class ChatControl : AControl
    {
        private const int MaxChatMessageLength = 96;

        private TextEntry m_TextEntry;
        private List<ChatLineTimed> m_TextEntries;
        private List<Tuple<ChatMode, string>> m_MessageHistory;

        private InputManager m_Input;
        private WorldModel m_World;

        private int m_MessageHistoryIndex = -1;

        private ChatMode m_Mode = ChatMode.Default;
        private ChatMode Mode
        {
            get { return m_Mode; }
            set
            {
                m_Mode = value;
                switch (value)
                {
                    case ChatMode.Default:
                        m_TextEntry.LeadingHtmlTag = string.Format("<outline color='#{0}' style='font-family: uni0;'>", 
                            Utility.GetColorFromUshort(Resources.HueData.GetHue(Settings.Game.SpeechColor, 1)));
                        m_TextEntry.LeadingText = string.Empty;
                        m_TextEntry.Text = string.Empty;
                        break;
                    case ChatMode.Whisper:
                        m_TextEntry.LeadingHtmlTag = string.Format("<outline color='#{0}' style='font-family: uni0;'>",
                            Utility.GetColorFromUshort(Resources.HueData.GetHue(Settings.Game.SpeechColor, 1)));
                        m_TextEntry.LeadingText = "Whisper: ";
                        m_TextEntry.Text = string.Empty;
                        break;
                    case ChatMode.Emote: // emote
                        m_TextEntry.LeadingHtmlTag = string.Format("<outline color='#{0}' style='font-family: uni0;'>",
                            Utility.GetColorFromUshort(Resources.HueData.GetHue(Settings.Game.EmoteColor, 1)));
                        m_TextEntry.LeadingText = "Emote: ";
                        m_TextEntry.Text = string.Empty;
                        break;
                    case ChatMode.Party: // party
                        m_TextEntry.LeadingHtmlTag = string.Format("<outline color='#{0}' style='font-family: uni0;'>",
                            Utility.GetColorFromUshort(Resources.HueData.GetHue(Settings.Game.PartyMsgColor, 1)));
                        m_TextEntry.LeadingText = "Party: ";
                        m_TextEntry.Text = string.Empty;
                        break;
                    case ChatMode.Guild: // guild
                        m_TextEntry.LeadingHtmlTag = string.Format("<outline color='#{0}' style='font-family: uni0;'>",
                            Utility.GetColorFromUshort(Resources.HueData.GetHue(Settings.Game.GuildMsgColor, 1)));
                        m_TextEntry.LeadingText = "Guild: ";
                        m_TextEntry.Text = string.Empty;
                        break;
                    case ChatMode.Alliance: // alliance
                        m_TextEntry.LeadingHtmlTag = string.Format("<outline color='#{0}' style='font-family: uni0;'>",
                            Utility.GetColorFromUshort(Resources.HueData.GetHue(Settings.Game.AllianceMsgColor, 1)));
                        m_TextEntry.LeadingText = "Alliance: ";
                        m_TextEntry.Text = string.Empty;
                        break;
                }
            }
        }

        public ChatControl(AControl parent, int x, int y, int width, int height)
            : base(parent)
        {
            Position = new Point(x, y);
            Size = new Point(width, height);

            m_TextEntries = new List<ChatLineTimed>();
            m_MessageHistory = new List<Tuple<ChatMode, string>>();

            m_Input = ServiceRegistry.GetService<InputManager>();
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
                Mode = ChatMode.Default;

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
            if (m_Input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.Q, false, false, true) && m_MessageHistoryIndex > -1)
            {
                if (m_MessageHistoryIndex > 0)
                    m_MessageHistoryIndex -= 1;
                {
                    Mode = m_MessageHistory[m_MessageHistoryIndex].Item1;
                    m_TextEntry.Text = m_MessageHistory[m_MessageHistoryIndex].Item2;
                }
            }
            else if (m_Input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.W, false, false, true))
            {
                if (m_MessageHistoryIndex < m_MessageHistory.Count - 1)
                {
                    m_MessageHistoryIndex += 1;
                    Mode = m_MessageHistory[m_MessageHistoryIndex].Item1;
                    m_TextEntry.Text = m_MessageHistory[m_MessageHistoryIndex].Item2;
                }
                else
                    m_TextEntry.Text = string.Empty;
            }
            // backspace when mode is not default and Text is empty = clear mode.
            else if (m_Input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.Back, false, false, false) && m_TextEntry.Text == string.Empty)
            {
                Mode = ChatMode.Default;
            }

            // if in default, only switch mode if there is a single command char (;, :, etc) followed by any other char.
            // in not in default, only switch mode if the single command char is the only char entered.
            if ((Mode == ChatMode.Default && m_TextEntry.Text.Length == 2) ||
                (Mode != ChatMode.Default && m_TextEntry.Text.Length == 1))
            {
                switch (m_TextEntry.Text[0])
                {
                    case ':': // emote
                        Mode = ChatMode.Emote;
                        break;
                    case ';': // whisper
                        Mode = ChatMode.Whisper;
                        break;
                    case '/': // party
                        Mode = ChatMode.Party;
                        break;
                    case '\\': // guild
                        Mode = ChatMode.Guild;
                        break;
                    case '|': // alliance
                        Mode = ChatMode.Alliance;
                        break;
                }
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

        public override void OnKeyboardReturn(int textID, string text)
        {
            m_TextEntry.Text = string.Empty;
            m_MessageHistory.Add(new Tuple<ChatMode, string>(Mode, text));
            m_MessageHistoryIndex = m_MessageHistory.Count;
            m_World.Interaction.SendSpeech(text, Mode);
            Mode = ChatMode.Default;
        }

        public void AddLine(string text, int font, int hue, bool asUnicode)
        {
            m_TextEntries.Add(new ChatLineTimed(string.Format("<outline style='font-family:{1}{2};'>{0}", text, asUnicode ? "uni" : "ascii", font), Width));
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
