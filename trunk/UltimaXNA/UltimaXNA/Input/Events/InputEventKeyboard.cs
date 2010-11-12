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
                WinKeys keys = (WinKeys)((int)_keyData & (int)WinKeys.KeyCode);

                if (!Enum.IsDefined(typeof(WinKeys), (int)keys))
                {
                    return WinKeys.None;
                }

                return keys;
            }
        }

        public int KeyValue
        {
            get { return (((int)_keyData) & 0xffff); }
        }

        /// <summary>
        /// Key to char helper conversion method.
        /// Note: If the keys are mapped other than on a default QWERTY
        /// keyboard, this method will not work properly. Most keyboards
        /// will return the same for A-Z and 0-9, but the special keys
        /// might be different.
        /// </summary>
        public char KeyChar
        {
            get
            {
                bool shiftPressed = this.Shift;
                char ret = ' ';
                WinKeys key = KeyCode;
                int keyNum = (int)key;
                if (keyNum >= (int)WinKeys.A && keyNum <= (int)WinKeys.Z)
                {
                    if (shiftPressed)
                        ret = key.ToString()[0];
                    else
                        ret = key.ToString().ToLower()[0];
                }
                else if (keyNum >= (int)WinKeys.D0 && keyNum <= (int)WinKeys.D9 &&
                    shiftPressed == false)
                {
                    ret = (char)((int)'0' + (keyNum - WinKeys.D0));
                }
                else if (keyNum >= (int)WinKeys.NumPad0 && keyNum <= (int)WinKeys.NumPad9)
                {
                    string name = Enum.GetName(typeof(WinKeys), key);
                    ret = name.Substring(name.Length - 1)[0];
                }
                else if (key == WinKeys.D1 && shiftPressed)
                    ret = '!';
                else if (key == WinKeys.D2 && shiftPressed)
                    ret = '@';
                else if (key == WinKeys.D3 && shiftPressed)
                    ret = '#';
                else if (key == WinKeys.D4 && shiftPressed)
                    ret = '$';
                else if (key == WinKeys.D5 && shiftPressed)
                    ret = '%';
                else if (key == WinKeys.D6 && shiftPressed)
                    ret = '^';
                else if (key == WinKeys.D7 && shiftPressed)
                    ret = '&';
                else if (key == WinKeys.D8 && shiftPressed)
                    ret = '*';
                else if (key == WinKeys.D9 && shiftPressed)
                    ret = '(';
                else if (key == WinKeys.D0 && shiftPressed)
                    ret = ')';
                else if (key == WinKeys.Oemtilde)
                    ret = shiftPressed ? '~' : '`';
                else if (key == WinKeys.OemMinus)
                    ret = shiftPressed ? '_' : '-';
                else if (key == WinKeys.OemPipe)
                    ret = shiftPressed ? '|' : '\\';
                else if (key == WinKeys.OemOpenBrackets)
                    ret = shiftPressed ? '{' : '[';
                else if (key == WinKeys.OemCloseBrackets)
                    ret = shiftPressed ? '}' : ']';
                else if (key == WinKeys.OemSemicolon)
                    ret = shiftPressed ? ':' : ';';
                else if (key == WinKeys.OemQuotes)
                    ret = shiftPressed ? '"' : '\'';
                else if (key == WinKeys.Oemcomma)
                    ret = shiftPressed ? '<' : '.';
                else if (key == WinKeys.OemPeriod)
                    ret = shiftPressed ? '>' : ',';
                else if (key == WinKeys.OemQuestion)
                    ret = shiftPressed ? '?' : '/';
                else if (key == WinKeys.Oemplus)
                    ret = shiftPressed ? '+' : '=';
                
                return ret;
            }
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
        Press,
    }
}
