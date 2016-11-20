/***************************************************************************
 *   TextEntryPage.cs
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

#region usings
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.UI;
using UltimaXNA.Core.UI.HTML;
using UltimaXNA.Core.Windows;
#endregion

namespace UltimaXNA.Ultima.UI.Controls
{
    class TextEntryPage : AControl
    {
        const float MSBetweenCaratBlinks = 500f;
        
        bool m_IsFocused;
        bool m_CaratBlinkOn;
        float m_MSSinceLastCaratBlink;
        RenderedText m_RenderedText;
        RenderedText m_RenderedCarat;
        int m_CaratAt;
        
        public string LeadingHtmlTag;
        public string Text;
        public int LineCount;
        public int EntryID;
        public bool InformParentOfReturnPress;

        public int CaratAt
        {
            get
            {
                if (m_CaratAt < 0)
                    m_CaratAt = 0;
                if (Text != null && m_CaratAt > Text.Length)
                    m_CaratAt = Text.Length;
                return m_CaratAt;
            }
            set
            {
                m_CaratAt = value;
                if (m_CaratAt < 0)
                    m_CaratAt = 0;
                if (m_CaratAt > Text.Length)
                    m_CaratAt = Text.Length;
            }
        }

        public override bool HandlesMouseInput => base.HandlesMouseInput & IsEditable;
        public override bool HandlesKeyboardFocus => base.HandlesKeyboardFocus & IsEditable;

        // ============================================================================================================
        // Ctors and BuildGumpling Methods
        // ============================================================================================================

        public TextEntryPage(AControl parent, int x, int y, int width, int height, int lineCount, int entryID)
            : this(parent)
        {
            BuildGumpling(x, y, width, height, lineCount, entryID);
        }

        TextEntryPage(AControl parent)
            : base(parent)
        {
            base.HandlesMouseInput = true;
            base.HandlesKeyboardFocus = true;
            IsEditable = true;
        }

        void BuildGumpling(int x, int y, int width, int height, int lineCount, int entryID)
        {
            Position = new Point(x, y);
            Size = new Point(width, height);
            LineCount = lineCount;
            EntryID = entryID;
            m_CaratBlinkOn = false;
            m_RenderedText = new RenderedText(string.Empty, Width, true);
            m_RenderedCarat = new RenderedText(string.Empty, 16, true);
        }

        // ============================================================================================================
        // Update and Draw
        // ============================================================================================================

        public override void Update(double totalMS, double frameMS)
        {
            if (UserInterface.KeyboardFocusControl == this)
            {
                // if we're not already focused, turn the carat on immediately.
                if (!m_IsFocused)
                {
                    m_IsFocused = true;
                    m_CaratBlinkOn = true;
                    m_MSSinceLastCaratBlink = 0f;
                }
                m_MSSinceLastCaratBlink += (float)frameMS;
                if (m_MSSinceLastCaratBlink >= MSBetweenCaratBlinks)
                {
                    m_MSSinceLastCaratBlink -= MSBetweenCaratBlinks;
                    if (m_CaratBlinkOn == true)
                    {
                        m_CaratBlinkOn = false;
                    }
                    else
                    {
                        m_CaratBlinkOn = true;
                    }
                }
            }
            else
            {
                m_IsFocused = false;
                m_CaratBlinkOn = false;
            }
            m_RenderedText.Text = $"{LeadingHtmlTag}{Text}";
            m_RenderedCarat.Text = $"{LeadingHtmlTag}|";
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            m_RenderedText.Draw(spriteBatch, new Rectangle(position.X, position.Y, Width, Height), 0, 0);
            if (IsEditable)
            {
                m_RenderedText.Draw(spriteBatch, position);
                if (m_CaratBlinkOn)
                {
                    Point caratPosition = m_RenderedText.Document.GetCaratPosition(m_CaratAt);
                    caratPosition.X += position.X;
                    caratPosition.Y += position.Y;
                    m_RenderedCarat.Draw(spriteBatch, caratPosition);
                }
            }
            base.Draw(spriteBatch, position);
        }

        // ============================================================================================================
        // Input
        // ============================================================================================================

        protected override void OnKeyboardInput(InputEventKeyboard e)
        {
            switch (e.KeyCode)
            {
                case WinKeys.Tab:
                    Parent.KeyboardTabToNextFocus(this);
                    break;
                case WinKeys.Enter:
                    if (InformParentOfReturnPress)
                    {
                        Parent.OnKeyboardReturn(EntryID, Text);
                    }
                    else
                    {
                        InsertCharacter(CaratAt, '\n');
                    }
                    break;
                case WinKeys.Back:
                    if (Text.Length > 0)
                    {
                        int escapedLength;
                        if (EscapeCharacters.TryFindEscapeCharacterBackwards(Text, CaratAt, out escapedLength))
                        {
                            Text = Text.Substring(0, Text.Length - escapedLength);
                            int carat = CaratAt - 1;
                            string before = (CaratAt == 0) ? null : Text.Substring(0, CaratAt - 1);
                            string after = Text.Substring(CaratAt);
                            Text = before + after;
                            CaratAt = carat;
                        }
                        else
                        {
                            int carat = CaratAt - 1;
                            string before = (CaratAt == 0) ? null : Text.Substring(0, CaratAt - 1);
                            string after = Text.Substring(CaratAt);
                            Text = before + after;
                            CaratAt = carat;
                        }
                    }
                    break;
                case WinKeys.Left:
                    CaratAt -= 1;
                    break;
                case WinKeys.Right:
                    CaratAt += 1;
                    break;
                default:
                    if (e.IsChar && e.KeyChar >= 32)
                    {
                        InsertCharacter(CaratAt, e.KeyChar);
                    }
                    break;
            }
        }

        void InsertCharacter(int index, char ch)
        {
            string escapedCharacter;
            int caratAt = index;
            string text = Text != null ? Text : string.Empty;
            if (EscapeCharacters.TryMatchChar(ch, out escapedCharacter))
            {
                text = text.Insert(CaratAt, escapedCharacter);
                caratAt += escapedCharacter.Length;
            }
            else
            {
                text = text.Insert(CaratAt, ch.ToString());
                caratAt += 1;
            }
            Text = text;
            CaratAt = caratAt;
        }
    }
}