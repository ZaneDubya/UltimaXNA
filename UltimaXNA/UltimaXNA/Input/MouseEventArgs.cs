using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace UltimaXNA.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class MouseEventArgs : EventArgs
    {
        private readonly MouseButton _button;
        private readonly int _clicks;
        private readonly int _delta;
        private readonly int _x;
        private readonly int _y;
        private bool _handled;

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
        public MouseButton Button
        {
            get { return _button; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Clicks
        {
            get { return _clicks; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Delta
        {
            get { return _delta; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int X
        {
            get { return _x; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Y
        {
            get { return _y; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector2 Position
        {
            get { return new Vector2(_x, _y); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <param name="clicks"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="delta"></param>
        public MouseEventArgs(MouseButton button, int clicks, int x, int y, int delta)
        {
            _button = button;
            _clicks = clicks;
            _x = x;
            _y = y;
            _delta = delta;
        }
    }
}
