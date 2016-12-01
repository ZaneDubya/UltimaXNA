/***************************************************************************
 *   InputEvent.cs
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
    public class InputEvent
    {
        readonly WinKeys m_Modifiers;
        bool m_Handled;

        public bool Alt => (m_Modifiers & WinKeys.Alt) == WinKeys.Alt;

        public bool Control => (m_Modifiers & WinKeys.Control) == WinKeys.Control;

        public bool Shift => (m_Modifiers & WinKeys.Shift) == WinKeys.Shift;

        public bool Handled
        {
            get { return m_Handled; }
            set { m_Handled = value; }
        }

        public InputEvent(WinKeys modifiers)
        {
            m_Modifiers = modifiers;
        }

        protected InputEvent(InputEvent parent)
        {
            m_Modifiers = parent.m_Modifiers;
        }
    }
}
