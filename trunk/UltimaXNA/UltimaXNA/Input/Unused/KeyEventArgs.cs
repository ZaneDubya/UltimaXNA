using System;

namespace UltimaXNA.Input.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class KeyEventArgs : EventArgs
    {
        private bool _handled;
        private readonly WinKeys _keyData;
        private bool _suppressKeyPress;

        /// <summary>
        /// 
        /// </summary>
        public virtual bool Alt
        {
            get { return ((_keyData & WinKeys.Alt) == WinKeys.Alt); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Control
        {
            get { return ((_keyData & WinKeys.Control) == WinKeys.Control); }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool Shift
        {
            get { return ((_keyData & WinKeys.Shift) == WinKeys.Shift); }
        }

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

        /// <summary>
        /// 
        /// </summary>
        public WinKeys KeyData
        {
            get { return _keyData; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int KeyValue
        {
            get { return (((int)_keyData) & 0xffff); }
        }

        /// <summary>
        /// 
        /// </summary>
        public WinKeys Modifiers
        {
            get { return (_keyData & ~WinKeys.KeyCode); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SuppressKeyPress
        {
            get { return _suppressKeyPress; }
            set
            {
                _suppressKeyPress = value;
                _handled = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyData"></param>
        public KeyEventArgs(WinKeys keyData)
        {
            _keyData = keyData;
        }
    }
}