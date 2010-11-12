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
        public MouseEvent EventType
        {
            get { return _eventType; }
        }

        protected new EventArgsMouse _args
        {
            get { return (EventArgsMouse)base._args; }
        }

        public MouseButton Button
        {
            get { return _args.Button; }
        }

        public Vector2 Position
        {
            get { return new Vector2(_args.X, _args.Y); }
        }

        private const int WHEEL_DELTA = 120;
        public int WheelValue
        {
            get { return (_args.Clicks / WHEEL_DELTA); }
        }

        public InputEventMouse(MouseEvent eventType, EventArgsMouse args) // WinKeys keyData, int x, int y, MouseButton button, 
            : base(args)
        {
            _eventType = eventType;
        }
    }

    public enum MouseEvent
    {
        Move,
        Down,
        Up,
        Click,
        DoubleClick,
        WheelScroll
    }
}
