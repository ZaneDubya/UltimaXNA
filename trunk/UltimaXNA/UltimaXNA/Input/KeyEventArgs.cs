using System;

namespace UltimaXNA.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class KeyEventArgs : EventArgs
    {
        private bool _handled;
        private readonly Keys _keyData;
        private bool _suppressKeyPress;

        /// <summary>
        /// 
        /// </summary>
        public virtual bool Alt
        {
            get { return ((_keyData & Keys.Alt) == Keys.Alt); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Control
        {
            get { return ((_keyData & Keys.Control) == Keys.Control); }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool Shift
        {
            get { return ((_keyData & Keys.Shift) == Keys.Shift); }
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
        public Keys KeyCode
        {
            get
            {
                Keys keys = _keyData & Keys.KeyCode;

                if (!Enum.IsDefined(typeof(Keys), (int)keys))
                {
                    return Keys.None;
                }

                return keys;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Keys KeyData
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
        public Keys Modifiers
        {
            get { return (_keyData & ~Keys.KeyCode); }
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
        public KeyEventArgs(Keys keyData)
        {
            _keyData = keyData;
        }
    }
}