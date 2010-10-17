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

        public WinKeys KeyCode
        {
            get
            {
                WinKeys keys = _keyData & WinKeys.KeyCode;

                if (!Enum.IsDefined(typeof(WinKeys), (int)keys))
                {
                    return WinKeys.None;
                }

                return keys;
            }
        }

        public WinKeys KeyData
        {
            get { return _keyData; }
        }

        public int KeyValue
        {
            get { return (((int)_keyData) & 0xffff); }
        }

        public char KeyChar
        {
            get { return (char)((ushort)KeyValue); }
        }

        public InputEventKeyboard(WinKeys keyData, KeyboardEvent eventType)
            : base(keyData)
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
