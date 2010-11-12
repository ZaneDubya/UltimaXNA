using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Input.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class KeyPressEventArgs : EventArgs
    {
        private char _keyChar;

        public char KeyChar
        {
            get { return _keyChar; }
            set { _keyChar = value; }
        }

        public KeyPressEventArgs(char keyChar)
        {
            _keyChar = keyChar;
        }
    }
}
