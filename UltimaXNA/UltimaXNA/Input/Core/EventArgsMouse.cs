using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace UltimaXNA.Input
{
    public class EventArgsMouse : EventArgs
    {
        private readonly MouseButtonInternal _button;
        private readonly int _clicks;
        private readonly int _mouseData;
        private readonly int _x;
        private readonly int _y;

        public MouseButtonInternal ButtonInternal
        {
            get { return _button; }
        }

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

        public Point2D Position
        {
            get { return new Point2D(_x, _y); }
        }

        public EventArgsMouse(MouseButtonInternal button, int clicks, int x, int y, int mouseData, WinKeys modifiers)
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
