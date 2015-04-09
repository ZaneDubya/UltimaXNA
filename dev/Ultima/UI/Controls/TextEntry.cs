/***************************************************************************
 *   TextEntry.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Input.Windows;
using Microsoft.Xna.Framework;
using System;
using UltimaXNA.Core.Graphics;
#endregion

namespace UltimaXNA.Ultima.UI.Controls
{
    class TextEntry : AControl
    {
        public int Hue = 0;
        public int EntryID = 0;
        public int LimitSize = 0;
        public bool IsPasswordField = false;

        string m_text = string.Empty;
        public string Text
        {
            get { return m_text; }
            set
            {
                m_text = value;
            }
        }

        public string HtmlTag = string.Empty;

        bool m_legacyCarat = false;
        public bool LegacyCarat { get { return m_legacyCarat; } set { m_legacyCarat = value; } }

        bool m_isFocused = false;
        bool m_caratBlinkOn = false;
        float m_MSSinceLastCaratBlink = 0f;
        const float c_MSBetweenCaratBlinks = 500f;

        RenderedText m_Texture;
        RenderedText m_Carat;

        public TextEntry(AControl owner, int page)
            : base(owner, page)
        {
            HandlesMouseInput = true;
            HandlesKeyboardFocus = true;
        }

        public TextEntry(AControl owner, int page, string[] arguements, string[] lines)
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

        public TextEntry(AControl owner, int page, int x, int y, int width, int height, int hue, int entryID, int limitSize, string text)
            : this(owner, page)
        {
            buildGumpling(x, y, width, height, hue, entryID, limitSize, text);
        }

        void buildGumpling(int x, int y, int width, int height, int hue, int entryID, int limitSize, string text)
        {
            Position = new Point(x, y);
            Size = new Point(width, height);
            Hue = hue;
            EntryID = entryID;
            Text = text;
            LimitSize = limitSize;
            m_caratBlinkOn = false;
            m_Texture = new RenderedText("", true, width);
            m_Carat = new RenderedText("", true, width);
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (Engine.UserInterface.KeyboardFocusControl == this)
            {
                // if we're not already focused, turn the carat on immediately.
                // if we're using the legacy carat, keep it visible. Else blink it every x seconds.
                if (!m_isFocused)
                {
                    m_isFocused = true;
                    m_caratBlinkOn = true;
                    m_MSSinceLastCaratBlink = 0f;
                }
                if (m_legacyCarat)
                    m_caratBlinkOn = true;
                else
                {
                    m_MSSinceLastCaratBlink += ((float)frameMS);
                    if (m_MSSinceLastCaratBlink >= c_MSBetweenCaratBlinks)
                    {
                        m_MSSinceLastCaratBlink = 0;
                        if (m_caratBlinkOn == true)
                            m_caratBlinkOn = false;
                        else
                            m_caratBlinkOn = true;
                    }
                }
            }
            else
            {
                m_isFocused = false;
                m_caratBlinkOn = false;
            }

            m_Texture.Text = HtmlTag + (IsPasswordField ? new string('*', Text.Length) : Text);
            m_Carat.Text = HtmlTag + (m_legacyCarat ? "_" : "|");

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            m_Texture.Draw(spriteBatch, Position);
            if (m_caratBlinkOn)
            {
                m_Carat.Draw(spriteBatch, new Point(X + m_Texture.Width, Y));
            }
            
            base.Draw(spriteBatch);
        }

        protected override void keyboardInput(InputEventKeyboard e)
        {
            switch (e.KeyCode)
            {
                case WinKeys.Back:
                    if (Text.Length > 0)
                    {
                        int escapedLength;
                        if (HTML.EscapeCharacters.TryFindEscapeCharacterBackwards(Text, Text.Length - 1, out escapedLength))
                        {
                            Text = Text.Substring(0, Text.Length - escapedLength);
                        }
                        else
                        {
                            Text = Text.Substring(0, Text.Length - 1);
                        }
                    }
                    break;
                case WinKeys.Tab:
                    m_owner.KeyboardTabToNextFocus(this);
                    break;
                case WinKeys.Enter:
                    m_owner.ActivateByKeyboardReturn(EntryID, Text);
                    break;
                default:
                    if (e.IsChar)
                    {
                        string escapedCharacter;
                        if (HTML.EscapeCharacters.TryMatchChar(e.KeyChar, out escapedCharacter))
                        {
                            Text += escapedCharacter;
                        }
                        else
                        {
                            Text += e.KeyChar;
                        }
                    }
                    break;
            }
        }
    }
}
