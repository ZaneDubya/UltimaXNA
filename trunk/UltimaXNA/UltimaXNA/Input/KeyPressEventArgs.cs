using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class KeyPressEventArgs : EventArgs
    {
        private bool _handled;
        private char _keyChar;

        /// <summary>
        /// 
        /// </summary>
        public bool Handled
        {
            get { return _handled; }
            set { _handled = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public char KeyChar
        {
            get { return _keyChar; }
            set { _keyChar = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyChar"></param>
        public KeyPressEventArgs(char keyChar)
        {
            _keyChar = keyChar;
        }
    }
}
