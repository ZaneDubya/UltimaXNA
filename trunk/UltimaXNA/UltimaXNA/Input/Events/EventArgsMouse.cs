using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace UltimaXNA.Input.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class EventArgsMouse : EventArgs
    {
        private readonly MouseButton _button;
        private readonly int _clicks;
        private readonly int _mouseData;
        private readonly int _x;
        private readonly int _y;

        public MouseButton Button
        {
            get { return _button; }
        }

        public int Clicks
        {
            get { return _clicks; }
        }

        public int MouseData
        {
            get { return _mouseData; }
        }

        public int X
        {
            get { return _x; }
        }

        public int Y
        {
            get { return _y; }
        }

        public Vector2 Position
        {
            get { return new Vector2(_x, _y); }
        }

        public EventArgsMouse(MouseButton button, int clicks, int x, int y, int mouseData, WinKeys modifiers)
            : base(modifiers)
        {
            _button = button;
            _clicks = clicks;
            _x = x;
            _y = y;
            _mouseData = mouseData;
        }
    }
}
