using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Input.Events
{
    public class InputEventKeyboard : InputEvent
    {
        protected readonly KeyboardEvent _eventType;
        public KeyboardEvent EventType
        {
            get { return _eventType; }
        }

        protected new EventArgsKeyboard _args
        {
            get { return (EventArgsKeyboard)base._args; }
        }

        WinKeys _keyCode = WinKeys.None;
        public WinKeys KeyCode
        {
            get
            {
                if (_keyCode != WinKeys.None)
                    return _keyCode;
                else
                    return _args.KeyCode;
            }

            set { _keyCode = value; }
        }

        public char KeyChar
        {
            get { return (char)KeyCode; }
        }

        public InputEventKeyboard(KeyboardEvent eventType, EventArgsKeyboard args)
            : base(args)
        {
            _eventType = eventType;
        }
    }

    public enum KeyboardEvent
    {
        Down,
        Up,
        Press
    }
}
