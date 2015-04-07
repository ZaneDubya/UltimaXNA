using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace UltimaXNA.Input.Windows
{
    public class InputEventMouse : InputEvent
    {
        public bool IsEvent(MouseEvent e, MouseButton b)
        {
            if (e == this.EventType && b == this.Button)
                return true;
            return false;
        }

        private readonly MouseEvent m_eventType;
        public MouseEvent EventType
        {
            get { return m_eventType; }
        }

        private const int WHEEL_DELTA = 120;
        public int WheelValue
        {
            get { return (m_clicks / WHEEL_DELTA); }
        }

        private readonly MouseButtonInternal m_button;
        private readonly int m_clicks;
        private readonly int m_mouseData;
        private readonly int m_x;
        private readonly int m_y;

        public MouseButton Button
        {
            get
            {
                if ((m_button & MouseButtonInternal.Left) == MouseButtonInternal.Left)
                    return MouseButton.Left;
                if ((m_button & MouseButtonInternal.Right) == MouseButtonInternal.Right)
                    return MouseButton.Right;
                if ((m_button & MouseButtonInternal.Middle) == MouseButtonInternal.Middle)
                    return MouseButton.Middle;
                if ((m_button & MouseButtonInternal.XButton1) == MouseButtonInternal.XButton1)
                    return MouseButton.XButton1;
                if ((m_button & MouseButtonInternal.XButton2) == MouseButtonInternal.XButton2)
                    return MouseButton.XButton2;
                return MouseButton.None;
            }
        }

        public int MouseData
        {
            get { return m_mouseData; }
        }

        public int X
        {
            get { return m_x; }
        }

        public int Y
        {
            get { return m_y; }
        }

        public Point Position
        {
            get { return new Point(m_x, m_y); }
        }

        public InputEventMouse(MouseEvent eventType, MouseButtonInternal button, int clicks, int x, int y, int mouseData, WinKeys modifiers)
            : base(modifiers)
        {
            m_eventType = eventType;
            m_button = button;
            m_clicks = clicks;
            m_x = (int)(x / UltimaVars.EngineVars.ScreenDPI.X);
            m_y = (int)(y / UltimaVars.EngineVars.ScreenDPI.Y);
            m_mouseData = mouseData;
        }

        public InputEventMouse(MouseEvent eventType, InputEventMouse parent)
            : base(parent)
        {
            m_eventType = eventType;
            m_button = parent.m_button;
            m_clicks = parent.m_clicks;
            m_x = parent.m_x;
            m_y = parent.m_y;
            m_mouseData = parent.m_mouseData;
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
