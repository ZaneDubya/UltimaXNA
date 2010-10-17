using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Input.Events
{
    public class InputEvent
    {
        protected bool _handled;
        protected readonly WinKeys _keyData;

        public virtual bool Alt
        {
            get { return ((_keyData & WinKeys.Alt) == WinKeys.Alt); }
        }

        public bool Control
        {
            get { return ((_keyData & WinKeys.Control) == WinKeys.Control); }
        }

        public virtual bool Shift
        {
            get { return ((_keyData & WinKeys.Shift) == WinKeys.Shift); }
        }

        public bool Handled
        {
            get { return _handled; }
            set { _handled = value; }
        }

        public WinKeys Modifiers
        {
            get { return (_keyData & ~WinKeys.KeyCode); }
        }

        public InputEvent(WinKeys keyData)
        {
            _keyData = keyData;
        }

        public void SuppressEvent()
        {
            _handled = true;
        }
    }
}
