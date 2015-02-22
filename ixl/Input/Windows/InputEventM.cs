using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace InterXLib.Input.Windows
{
    public class InputEventMouse : InputEvent
    {
        public bool IsEvent(MouseEvent e, MouseButton b)
        {
            if (e == this.EventType && b == this.Button)
                return true;
            return false;
        }

        private readonly MouseEvent _eventType;
        public MouseEvent EventType
        {
            get { return _eventType; }
        }

        private const int WHEEL_DELTA = 120;
        public int WheelValue
        {
            get { return (_clicks / WHEEL_DELTA); }
        }

        private readonly MouseButtonInternal _button;
        private readonly int _clicks;
        private readonly int _mouseData;
        private readonly int _x;
        private readonly int _y;

        public MouseButton Button
        {
            get
            {
                if ((_button & MouseButtonInternal.Left) == MouseButtonInternal.Left)
                    return MouseButton.Left;
                if ((_button & MouseButtonInternal.Right) == MouseButtonInternal.Right)
                    return MouseButton.Right;
                if ((_button & MouseButtonInternal.Middle) == MouseButtonInternal.Middle)
                    return MouseButton.Middle;
                if ((_button & MouseButtonInternal.XButton1) == MouseButtonInternal.XButton1)
                    return MouseButton.XButton1;
                if ((_button & MouseButtonInternal.XButton2) == MouseButtonInternal.XButton2)
                    return MouseButton.XButton2;
                return MouseButton.None;
            }
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

        public Point Position
        {
            get { return new Point(_x, _y); }
        }

        public InputEventMouse(MouseEvent eventType, MouseButtonInternal button, int clicks, int x, int y, int mouseData, WinKeys modifiers)
            : base(modifiers)
        {
            _eventType = eventType;
            _button = button;
            _clicks = clicks;
            _x = (int)(x / Settings.ScreenDPI.X);
            _y = (int)(y / Settings.ScreenDPI.Y);
            _mouseData = mouseData;
        }

        public InputEventMouse(MouseEvent eventType, InputEventMouse parent)
            : base(parent)
        {
            _eventType = eventType;
            _button = parent._button;
            _clicks = parent._clicks;
            _x = parent._x;
            _y = parent._y;
            _mouseData = parent._mouseData;
        }
    }

    public enum MouseEvent
    {
        Move,
        Down,
        Up,
        WheelScroll,
        DragBegin,
        DragEnd,
        Click,
        DoubleClick,
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
