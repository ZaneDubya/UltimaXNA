using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Input.Unused
{
    public class InputBinding
    {
        private static object _syncRoot = new object();

        private string _name;
        private bool _shift;
        private bool _alt;
        private bool _control;
        private bool _isExecuting;

        public WinKeys ModifierKeys
        {
            get
            {
                WinKeys modifiers = WinKeys.None;

                modifiers |= _shift ? WinKeys.Shift : modifiers;
                modifiers |= _alt ? WinKeys.Alt : modifiers;
                modifiers |= _control ? WinKeys.Control : modifiers;

                return modifiers;
            }
        }

        public bool Shift
        {
            get { return _shift; }
        }

        public bool Alt
        {
            get { return _alt; }
        }

        public bool Control
        {
            get { return _control; }
        }

        public string Name
        {
            get { return _name; }
        }

        public bool IsExecuting
        {
            get { return _isExecuting; }
            internal set
            {
                if (value && !_isExecuting)
                {
                    OnBeginExecution();
                }
                else if (value && _isExecuting && EndExecution == null)
                {
                    OnBeginExecution();
                }
                else if (!value && _isExecuting)
                {
                    OnEndExecution();
                }
                else { } //Is there another scenario?

                _isExecuting = value;
            }
        }

        public EventHandler BeginExecution;
        public EventHandler EndExecution;

        public InputBinding(string name, bool shift, bool control, bool alt)
        {
            _name = name;
            _shift = shift;
            _alt = alt;
            _control = control;
        }

        protected virtual void OnBeginExecution()
        {
            if (BeginExecution != null)
            {
                BeginExecution(this, EventArgs.Empty);
            }
        }

        protected virtual void OnEndExecution()
        {
            if (EndExecution != null)
            {
                EndExecution(this, EventArgs.Empty);
            }
        }
    }
}
