using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UltimaXNA.Input.Events;

namespace UltimaXNA.Input
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

        public Point2D Position
        {
            get { return _args.Position; }
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
        WheelScroll,
        DragBegin,
        DragEnd
    }

    public enum MouseButton
    {
        Left = 0,
        Right = 1,
        Middle = 2,
        XButton1 = 3,
        XButton2 = 4,
        None = 0x7f
    }
}
