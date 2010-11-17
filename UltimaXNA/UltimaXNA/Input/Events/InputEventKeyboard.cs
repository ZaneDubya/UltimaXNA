using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Input.Events;

namespace UltimaXNA.Input
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

        public WinKeys KeyCode
        {
            get { return _args.KeyCode; }
        }

        WinKeys _keyChar = WinKeys.None;
        public char KeyChar
        {
            get
            {
                if (_keyChar != WinKeys.None)
                    return (char)_keyChar;
                else
                    return '\0';
            }
        }

        public bool IsChar
        {
            get { return KeyChar != '\0'; }
        }

        public void OverrideKeyChar(WinKeys newChar) { _keyChar = newChar; }

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
