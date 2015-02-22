using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterXLib.Input.Windows
{
    public class InputEvent
    {
        protected readonly WinKeys _modifiers;
        protected bool _handled = false;

        public virtual bool Alt
        {
            get { return ((_modifiers & WinKeys.Alt) == WinKeys.Alt); }
        }

        public bool Control
        {
            get { return ((_modifiers & WinKeys.Control) == WinKeys.Control); }
        }

        public virtual bool Shift
        {
            get { return ((_modifiers & WinKeys.Shift) == WinKeys.Shift); }
        }

        public InputEvent(WinKeys modifiers)
        {
            _modifiers = modifiers;
        }

        protected InputEvent(InputEvent parent)
        {
            _modifiers = parent._modifiers;
        }

        public bool Handled
        {
            get { return _handled; }
            set { _handled = value; }
        }

        public void SuppressEvent()
        {
            _handled = true;
        }
    }
}
