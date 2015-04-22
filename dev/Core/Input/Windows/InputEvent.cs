using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
