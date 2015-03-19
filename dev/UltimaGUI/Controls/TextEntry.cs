/***************************************************************************
 *   TextEntry.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using Microsoft.Xna.Framework;
using UltimaXNA.Rendering;
using InterXLib.Input.Windows;
using UltimaXNA.UltimaGUI;

namespace UltimaXNA.UltimaGUI.Controls
{
    class TextEntry : Control
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
        float m_secondsSinceLastBlink = 0f;
        const float m_SecondsPerBlink = 0.5f;

        UltimaGUI.TextRenderer m_textRenderer;
        UltimaGUI.TextRenderer m_caratRenderer;

        public TextEntry(Control owner, int page)
            : base(owner, page)
        {
            HandlesMouseInput = true;
            HandlesKeyboardFocus = true;
        }

        public TextEntry(Control owner, int page, string[] arguements, string[] lines)
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

        public TextEntry(Control owner, int page, int x, int y, int width, int height, int hue, int entryID, int limitSize, string text)
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
            m_textRenderer = new UltimaGUI.TextRenderer("", width, true);
            m_caratRenderer = new UltimaGUI.TextRenderer("", width, true);
        }

        public override void Update(GameTime gameTime)
        {
            if (UserInterface.KeyboardFocusControl == this)
            {
                // if we're not already focused, turn the carat on immediately.
                // if we're using the legacy carat, keep it visible. Else blink it every x seconds.
                if (!m_isFocused)
                {
                    m_isFocused = true;
                    m_caratBlinkOn = true;
                    m_secondsSinceLastBlink = 0f;
                }
                if (m_legacyCarat)
                    m_caratBlinkOn = true;
                else
                {
                    m_secondsSinceLastBlink += ((float)gameTime.ElapsedGameTime.TotalSeconds);
                    if (m_secondsSinceLastBlink >= m_SecondsPerBlink)
                    {
                        m_secondsSinceLastBlink -= m_SecondsPerBlink;
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

            m_textRenderer.Text = HtmlTag + (IsPasswordField ? new string('*', Text.Length) : Text);
            m_caratRenderer.Text = HtmlTag + (m_legacyCarat ? "_" : "|");

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            m_textRenderer.Draw(spriteBatch, Position);
            if (m_caratBlinkOn)
                m_caratRenderer.Draw(spriteBatch, new Point(X + m_textRenderer.Width, Y));
            
            base.Draw(spriteBatch);
        }

        protected override void keyboardInput(InputEventKeyboard e)
        {
            switch (e.KeyCode)
            {
                case WinKeys.Back:
                    if (Text.Length > 0)
                    {
                        Text = Text.Substring(0, Text.Length - 1);
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
                        Text += e.KeyChar;
                    }
                    break;
            }
        }
    }
}
