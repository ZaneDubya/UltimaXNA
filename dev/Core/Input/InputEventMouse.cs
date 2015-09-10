/***************************************************************************
 *   InputEventMouse.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using Microsoft.Xna.Framework;
using UltimaXNA.Core.Windows;

namespace UltimaXNA.Core.Input
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

        private readonly WinMouseButtons m_button;
        private readonly int m_clicks;
        private readonly int m_mouseData;
        private readonly int m_x;
        private readonly int m_y;

        public MouseButton Button
        {
            get
            {
                if ((m_button & WinMouseButtons.Left) == WinMouseButtons.Left)
                    return MouseButton.Left;
                if ((m_button & WinMouseButtons.Right) == WinMouseButtons.Right)
                    return MouseButton.Right;
                if ((m_button & WinMouseButtons.Middle) == WinMouseButtons.Middle)
                    return MouseButton.Middle;
                if ((m_button & WinMouseButtons.XButton1) == WinMouseButtons.XButton1)
                    return MouseButton.XButton1;
                if ((m_button & WinMouseButtons.XButton2) == WinMouseButtons.XButton2)
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

        public InputEventMouse(MouseEvent eventType, WinMouseButtons button, int clicks, int x, int y, int mouseData, WinKeys modifiers)
            : base(modifiers)
        {
            Vector2 dpi = DpiManager.GetSystemDpiScalar();

            m_eventType = eventType;
            m_button = button;
            m_clicks = clicks;
            m_x = (int)(x / dpi.X);
            m_y = (int)(y / dpi.Y);
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
}
