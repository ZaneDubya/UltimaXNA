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
using System;
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.UI;
using UltimaXNA.Core.UI.HTML;
#endregion

namespace UltimaXNA.Ultima.UI.Controls
{
    class TextEntry : AControl
    {
        public int Hue = 0;
        public int EntryID = 0;
        public int LimitSize = 0;
        public bool IsPasswordField = false;
        public bool ReplaceDefaultTextOnFirstKeypress = false;
        public bool NumericOnly = false;
        public string HtmlTag = string.Empty;

        public string Text
        {
            get;
            set;
        }

        public bool LegacyCarat
        {
            get;
            set;
        }

        private bool m_IsFocused = false;
        private bool m_CaratBlinkOn = false;
        private float m_MSSinceLastCaratBlink = 0f;
        private const float c_MSBetweenCaratBlinks = 500f;

        private RenderedText m_RenderedText;
        private RenderedText m_Carat;

        private UserInterfaceService m_UserInterface;

        public TextEntry(AControl parent)
            : base(parent)
        {
            HandlesMouseInput = true;
            HandlesKeyboardFocus = true;

            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
        }

        public TextEntry(AControl parent, string[] arguements, string[] lines)
            : this(parent)
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

        public TextEntry(AControl parent, int x, int y, int width, int height, int hue, int entryID, int limitSize, string text)
            : this(parent)
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
            m_CaratBlinkOn = false;
            m_RenderedText = new RenderedText(string.Empty, 1024);
            m_Carat = new RenderedText(string.Empty, 16);
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_UserInterface.KeyboardFocusControl == this)
            {
                // if we're not already focused, turn the carat on immediately.
                // if we're using the legacy carat, keep it visible. Else blink it every x seconds.
                if (!m_IsFocused)
                {
                    m_IsFocused = true;
                    m_CaratBlinkOn = true;
                    m_MSSinceLastCaratBlink = 0f;
                }
                if (LegacyCarat)
                {
                    m_CaratBlinkOn = true;
                }
                else
                {
                    m_MSSinceLastCaratBlink += ((float)frameMS);
                    if (m_MSSinceLastCaratBlink >= c_MSBetweenCaratBlinks)
                    {
                        m_MSSinceLastCaratBlink = 0;
                        if (m_CaratBlinkOn == true)
                            m_CaratBlinkOn = false;
                        else
                            m_CaratBlinkOn = true;
                    }
                }
            }
            else
            {
                m_IsFocused = false;
                m_CaratBlinkOn = false;
            }

            m_RenderedText.Text = HtmlTag + (IsPasswordField ? new string('*', Text.Length) : Text);
            m_Carat.Text = HtmlTag + (LegacyCarat ? "_" : "|");

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            Point caratPosition = new Point(position.X, position.Y);

            if (m_RenderedText.Width + m_Carat.Width <= Width)
            {
                m_RenderedText.Draw(spriteBatch, position, Utility.GetHueVector(Hue));
                caratPosition.X += m_RenderedText.Width;
            }
            else
            {
                int textOffset = m_RenderedText.Width - (Width - m_Carat.Width);
                m_RenderedText.Draw(spriteBatch, new Rectangle(position.X, position.Y, m_RenderedText.Width - textOffset, m_RenderedText.Height), textOffset, 0, Utility.GetHueVector(Hue));
                caratPosition.X += (Width - m_Carat.Width);
            }


            if (m_CaratBlinkOn)
                m_Carat.Draw(spriteBatch, caratPosition, Utility.GetHueVector(Hue));
            base.Draw(spriteBatch, position);
        }

        protected override void OnKeyboardInput(InputEventKeyboard e)
        {
            switch (e.KeyCode)
            {
                case WinKeys.Tab:
                    Parent.KeyboardTabToNextFocus(this);
                    break;
                case WinKeys.Enter:
                    Parent.ActivateByKeyboardReturn(EntryID, Text);
                    break;
                case WinKeys.Back:
                    if (ReplaceDefaultTextOnFirstKeypress)
                    {
                        Text = string.Empty;
                        ReplaceDefaultTextOnFirstKeypress = false;
                    }
                    else if (Text.Length > 0)
                    {
                        int escapedLength;
                        if (EscapeCharacters.TryFindEscapeCharacterBackwards(Text, Text.Length - 1, out escapedLength))
                        {
                            Text = Text.Substring(0, Text.Length - escapedLength);
                        }
                        else
                        {
                            Text = Text.Substring(0, Text.Length - 1);
                        }
                    }
                    break;
                default:
                    if (NumericOnly && !char.IsNumber(e.KeyChar))
                        return;

                    if (ReplaceDefaultTextOnFirstKeypress)
                    {
                        Text = string.Empty;
                        ReplaceDefaultTextOnFirstKeypress = false;
                    }

                    if (e.IsChar)
                    {
                        string escapedCharacter;
                        if (EscapeCharacters.TryMatchChar(e.KeyChar, out escapedCharacter))
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
