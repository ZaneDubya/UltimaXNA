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

namespace UltimaXNA.Core.Input.Windows
{
    public class InputEvent
    {
        protected readonly WinKeys m_modifiers;
        protected bool m_handled = false;

        public virtual bool Alt
        {
            get { return ((m_modifiers & WinKeys.Alt) == WinKeys.Alt); }
        }

        public bool Control
        {
            get { return ((m_modifiers & WinKeys.Control) == WinKeys.Control); }
        }

        public virtual bool Shift
        {
            get { return ((m_modifiers & WinKeys.Shift) == WinKeys.Shift); }
        }

        public InputEvent(WinKeys modifiers)
        {
            m_modifiers = modifiers;
        }

        protected InputEvent(InputEvent parent)
        {
            m_modifiers = parent.m_modifiers;
        }

        public bool Handled
        {
            get { return m_handled; }
            set { m_handled = value; }
        }

        public void SuppressEvent()
        {
            m_handled = true;
        }
    }
}
