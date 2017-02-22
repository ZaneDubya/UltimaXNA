﻿/***************************************************************************
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
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Core.Network;

#endregion usings

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    internal class ChatControl : AControl
    {
        private const int MaxChatMessageLength = 96;

        private TextEntry m_TextEntry;
        private List<ChatLineTimed> m_TextEntries;
        private List<Tuple<ChatMode, string>> m_MessageHistory;
        private IInputService m_Input;
        private WorldModel m_World;
        private int m_MessageHistoryIndex = -1;
        private Serial m_PrivateMessageSerial = Serial.Null;
        private string m_PrivateMessageName;

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
                            Utility.GetColorFromUshort(Resources.HueData.GetHue(Settings.UserInterface.SpeechColor, 1)));
                        m_TextEntry.LeadingText = string.Empty;
                        m_TextEntry.Text = string.Empty;
                        break;

                    case ChatMode.Whisper:
                        m_TextEntry.LeadingHtmlTag = string.Format("<outline color='#{0}' style='font-family: uni0;'>",
                            Utility.GetColorFromUshort(Resources.HueData.GetHue(Settings.UserInterface.SpeechColor, 1)));
                        m_TextEntry.LeadingText = "Whisper: ";
                        m_TextEntry.Text = string.Empty;
                        break;

                    case ChatMode.Emote:
                        m_TextEntry.LeadingHtmlTag = string.Format("<outline color='#{0}' style='font-family: uni0;'>",
                            Utility.GetColorFromUshort(Resources.HueData.GetHue(Settings.UserInterface.EmoteColor, 1)));
                        m_TextEntry.LeadingText = "Emote: ";
                        m_TextEntry.Text = string.Empty;
                        break;

                    case ChatMode.Party:
                        m_TextEntry.LeadingHtmlTag = string.Format("<outline color='#{0}' style='font-family: uni0;'>",
                            Utility.GetColorFromUshort(Resources.HueData.GetHue(Settings.UserInterface.PartyMsgColor, 1)));
                        m_TextEntry.LeadingText = "Party: ";
                        m_TextEntry.Text = string.Empty;
                        break;

                    case ChatMode.PartyPrivate:
                        m_TextEntry.LeadingHtmlTag = string.Format("<outline color='#{0}' style='font-family: uni0;'>",
                            Utility.GetColorFromUshort(Resources.HueData.GetHue(Settings.UserInterface.PartyPrivateMsgColor, 1)));
                        m_TextEntry.LeadingText = $"To {m_PrivateMessageName}: ";
                        m_TextEntry.Text = string.Empty;
                        break;

                    case ChatMode.Guild:
                        m_TextEntry.LeadingHtmlTag = string.Format("<outline color='#{0}' style='font-family: uni0;'>",
                            Utility.GetColorFromUshort(Resources.HueData.GetHue(Settings.UserInterface.GuildMsgColor, 1)));
                        m_TextEntry.LeadingText = "Guild: ";
                        m_TextEntry.Text = string.Empty;
                        break;

                    case ChatMode.Alliance:
                        m_TextEntry.LeadingHtmlTag = string.Format("<outline color='#{0}' style='font-family: uni0;'>",
                            Utility.GetColorFromUshort(Resources.HueData.GetHue(Settings.UserInterface.AllianceMsgColor, 1)));
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

            m_Input = Service.Get<IInputService>();
            m_World = Service.Get<WorldModel>();

            IsUncloseableWithRMB = true;
        }

        public void SetModeToPartyPrivate(string name, Serial serial)
        {
            m_PrivateMessageName = name;
            m_PrivateMessageSerial = serial;
            Mode = ChatMode.PartyPrivate;
        }

        public void Speech(string text)//we should add something like this
        {
            Mode = ChatMode.Default;
            OnKeyboardReturn(0, text);
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_TextEntry == null)
            {
                IResourceProvider provider = Service.Get<IResourceProvider>();
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

            // Ctrl-Q = Cycle backwards through the things you have said today Ctrl-W = Cycle
            // forwards through the things you have said today
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

            // only switch mode if the single command char is the only char entered.
            if ((Mode == ChatMode.Default && m_TextEntry.Text.Length == 1) ||
                (Mode != ChatMode.Default && m_TextEntry.Text.Length == 1))
            {
                switch (m_TextEntry.Text[0])
                {
                    case ':':
                        Mode = ChatMode.Emote;
                        break;

                    case ';':
                        Mode = ChatMode.Whisper;
                        break;

                    case '/':
                        Mode = ChatMode.Party;
                        break;

                    case '\\':
                        Mode = ChatMode.Guild;
                        break;

                    case '|':
                        Mode = ChatMode.Alliance;
                        break;
                }
            }

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            int y = m_TextEntry.Y + position.Y - 6;
            for (int i = m_TextEntries.Count - 1; i >= 0; i--)
            {
                y -= m_TextEntries[i].TextHeight;
                m_TextEntries[i].Draw(spriteBatch, new Point(position.X + 2, y));
            }
            base.Draw(spriteBatch, position, frameMS);
        }

        public override void OnKeyboardReturn(int textID, string text)
        {
            // local variables
            ChatMode sentMode = Mode;
            MessageTypes speechType = MessageTypes.Normal;
            int hue = 0;
            // save this message and reset chat for next entry
            m_TextEntry.Text = string.Empty;
            m_MessageHistory.Add(new Tuple<ChatMode, string>(Mode, text));
            m_MessageHistoryIndex = m_MessageHistory.Count;
            Mode = ChatMode.Default;
            // send the message and display it locally.
            switch (sentMode)
            {
                case ChatMode.Default:
                    speechType = MessageTypes.Normal;
                    hue = Settings.UserInterface.SpeechColor;
                    break;

                case ChatMode.Whisper:
                    speechType = MessageTypes.Whisper;
                    hue = Settings.UserInterface.SpeechColor;
                    break;

                case ChatMode.Emote:
                    speechType = MessageTypes.Emote;
                    hue = Settings.UserInterface.EmoteColor;
                    break;

                case ChatMode.Party:
                    PlayerState.Partying.DoPartyCommand(text);
                    return;

                case ChatMode.PartyPrivate:
                    PlayerState.Partying.SendPartyPrivateMessage(m_PrivateMessageSerial, text);
                    return;

                case ChatMode.Guild:
                    speechType = MessageTypes.Guild;
                    hue = Settings.UserInterface.GuildMsgColor;
                    break;

                case ChatMode.Alliance:
                    speechType = MessageTypes.Alliance;
                    hue = Settings.UserInterface.AllianceMsgColor;
                    break;
            }
            INetworkClient network = Service.Get<INetworkClient>();
            network.Send(new AsciiSpeechPacket(speechType, 0, hue + 2, "ENU", text));
        }

        public void AddLine(string text, int font, int hue, bool asUnicode)
        {
            m_TextEntries.Add(new ChatLineTimed(string.Format("<outline color='#{3}' style='font-family:{1}{2};'>{0}",
                text, asUnicode ? "uni" : "ascii", font, Utility.GetColorFromUshort(Resources.HueData.GetHue(hue, -1))),
                Width));
        }

        private class ChatLineTimed
        {
            private readonly string m_text;
            public string Text { get { return m_text; } }
            private float m_createdTime = float.MinValue;
            private bool m_isExpired;
            public bool IsExpired { get { return m_isExpired; } }
            private float m_alpha;
            public float Alpha { get { return m_alpha; } }
            private int m_width;

            private const float Time_Display = 10000.0f;
            private const float Time_Fadeout = 4000.0f;

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
                    m_createdTime = (float) totalMS;
                float time = (float) totalMS - m_createdTime;
                if (time > Time_Display)
                    m_isExpired = true;
                else if (time > (Time_Display - Time_Fadeout))
                {
                    m_alpha = 1.0f - ((time) - (Time_Display - Time_Fadeout)) / Time_Fadeout;
                }
            }

            public void Draw(SpriteBatchUI sb, Point position)
            {
                m_Texture.Draw(sb, position, Utility.GetHueVector(0, false, (m_alpha < 1.0f), true));
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
}