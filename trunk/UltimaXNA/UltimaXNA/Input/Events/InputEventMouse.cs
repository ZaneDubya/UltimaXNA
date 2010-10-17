using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace UltimaXNA.Input.Events
{
    public class InputEventMouse : InputEvent
    {
        private readonly MouseEvent _eventType;
        private readonly MouseButton _button;
        private readonly int _x;
        private readonly int _y;

        public MouseEvent EventType
        {
            get { return _eventType; }
        }

        public MouseButton Button
        {
            get { return _button; }
        }

        public Vector2 Position
        {
            get { return new Vector2(_x, _y); }
        }

        public InputEventMouse(WinKeys keyData, int x, int y, MouseButton button, MouseEvent eventType)
            : base(keyData)
        {
            _x = x;
            _y = y;
            _button = button;
            _eventType = eventType;
        }
    }

    public enum MouseEvent
    {
        Down,
        Up,
        Click,
        DoubleClick,
        WheelScrollUp,
        WheelScrollDown
    }
}
