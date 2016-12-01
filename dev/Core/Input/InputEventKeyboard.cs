/***************************************************************************
 *   InputEventKeyboard.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using UltimaXNA.Core.Windows;

namespace UltimaXNA.Core.Input
{
    public class InputEventKeyboard : InputEvent
    {
        public readonly KeyboardEvent EventType;
        public readonly WinKeys KeyCode;
        readonly int m_KeyDataExtra;
        WinKeys m_KeyChar = WinKeys.None;

        public char KeyChar
        {
            get
            {
                if (m_KeyChar != WinKeys.None)
                {
                    return CultureHandler.TranslateChar((char)m_KeyChar);
                }
                else
                {
                    return '\0';
                }
            }
        }

        public bool IsChar => KeyChar != '\0';

        public override string ToString() => $"{EventType} {KeyChar}";

        /// <summary>
        /// The repeat count for the current key. The value is the number of times the keystroke is autorepeated as
        /// a result of the user holding down the key. If the keystroke is held long enough, multiple messages are
        /// sent. However, the repeat count is not cumulative. The repeat count is always 1 for a WM_KEYUP message.
        /// </summary>
        public int DataRepeatCount => m_KeyDataExtra & 0x0000FFFF;

        /// <summary>
        /// Indicates whether the key is an extended key, such as the right-hand ALT and CTRL keys that appear on an
        /// enhanced 101- or 102-key keyboard. The value is 1 if it is an extended key; otherwise, it is 0.
        /// </summary>
        public int DataIsExtendedKey => (m_KeyDataExtra >> 24) & 0x00000001;

        /// <summary>
        /// The context code. The value is always 0 for a WM_KEYDOWN or a WM_KEYUP message. The value is 1 if the ALT
        /// key is down while the key is pressed; it is 0 if the WM_SYSKEYDOWN or WM_SYSKEYUP message is posted to the
        /// active window because no window has the keyboard focus.
        /// </summary>
        public int DataContextCode => (m_KeyDataExtra >> 29) & 0x00000001;

        /// <summary>
        /// The previous key state. The value is 1 if the key is down before the message is sent, or it is zero if the
        /// key is up.  The value is always 1 for a WM_(SYS)KEYUP message.
        /// </summary>
        public int DataPreviousState => (m_KeyDataExtra >> 30) & 0x00000001;

        /// <summary>
        /// The transition state. The value is always 0 for a WM_(SYS)KEYDOWN message.  The value is always 1 for a
        /// WM_(SYS)KEYUP message.
        /// </summary>
        public int DataTransitionState => (m_KeyDataExtra >> 31) & 0x00000001;

        public InputEventKeyboard(KeyboardEvent eventType, WinKeys virtKeyCode, int keyData, WinKeys modifiers)
            : base(modifiers)
        {
            EventType = eventType;
            KeyCode = virtKeyCode;
            m_KeyDataExtra = keyData;
        }

        public InputEventKeyboard(KeyboardEvent eventType, InputEventKeyboard parent)
            : base(parent)
        {
            EventType = eventType;
            KeyCode = parent.KeyCode;
            m_KeyDataExtra = parent.m_KeyDataExtra;
        }

        public void OverrideKeyChar(WinKeys newChar)
        {
            m_KeyChar = newChar;
        }
    }
}
