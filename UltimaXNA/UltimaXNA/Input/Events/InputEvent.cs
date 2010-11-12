using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Input.Events
{
    public class InputEvent
    {
        protected bool _handled;
        protected EventArgs _args;

        public bool Alt
        {
            get { return _args.Alt; }
        }

        public bool Control
        {
            get { return _args.Control; }
        }

        public bool Shift
        {
            get { return _args.Shift; }
        }

        public bool Handled
        {
            get { return _handled; }
            set { _handled = value; }
        }

        public InputEvent(EventArgs args)
        {
            _args = args;
        }

        public void SuppressEvent()
        {
            _handled = true;
        }
    }
}
