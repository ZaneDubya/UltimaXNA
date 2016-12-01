/***************************************************************************
 *   InputManager.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

#region usings
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Extensions;
using UltimaXNA.Core.Windows;
#endregion

namespace UltimaXNA.Core.Input
{
    public class InputService : IInputService
    {
        WndProc m_WndProc;
        List<InputEvent> m_Events = new List<InputEvent>();
        List<InputEvent> m_EventsNext = new List<InputEvent>();
        MouseState m_MouseState;
        MouseState m_MouseStateLast;

        // Mouse dragging support
        const int MouseDragBeginDistance = 2;
        const int MouseClickMaxDelta = 2;
        InputEventMouse m_LastMouseClick;
        float m_LastMouseClickTime;
        InputEventMouse m_LastMouseDown;
        float m_LastMouseDownTime;
        bool m_MouseIsDragging;
        float m_TheTime = -1f;

        public InputService(IntPtr handle)
        {
            m_WndProc = new WndProc(handle);
            m_WndProc.MouseWheel += AddEvent;
            m_WndProc.MouseMove += OnMouseMove;
            m_WndProc.MouseUp += OnMouseUp;
            m_WndProc.MouseDown += OnMouseDown;
            m_WndProc.KeyDown += OnKeyDown;
            m_WndProc.KeyUp += OnKeyUp;
            m_WndProc.KeyChar += OnKeyChar;
        }

        public void Dispose()
        {
            m_WndProc.MouseWheel -= AddEvent;
            m_WndProc.MouseMove -= OnMouseMove;
            m_WndProc.MouseUp -= OnMouseUp;
            m_WndProc.MouseDown -= OnMouseDown;
            m_WndProc.KeyDown -= OnKeyDown;
            m_WndProc.KeyUp -= OnKeyUp;
            m_WndProc.KeyChar -= OnKeyChar;
            m_WndProc.Dispose();
            m_WndProc = null;
        }

        public void Update(double totalTime, double frameTime)
        {
            m_TheTime = (float)totalTime;
            m_MouseStateLast = m_MouseState;
            m_MouseState = m_WndProc.MouseState.CreateWithDPI(DpiManager.GetSystemDpiScalar());
            lock (m_EventsNext)
            {
                m_Events.Clear();
                foreach (InputEvent e in m_EventsNext)
                {
                    m_Events.Add(e);
                }
                m_EventsNext.Clear();
            }
        }

        public bool IsCtrlDown => NativeMethods.GetKeyState((int)WinKeys.ControlKey) < 0;

        public bool IsShiftDown => NativeMethods.GetKeyState((int)WinKeys.ShiftKey) < 0;

        public bool IsKeyDown(WinKeys key) => NativeMethods.GetKeyState((int)key) < 0;

        public Point MousePosition => new Point(m_MouseState.X, m_MouseState.Y);

        public bool IsMouseButtonDown(MouseButton btn)
        {
            switch (btn)
            {
                case MouseButton.Left:
                    return m_MouseState.LeftButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return m_MouseState.MiddleButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return m_MouseState.RightButton == ButtonState.Pressed;
                case MouseButton.XButton1:
                    return m_MouseState.XButton1 == ButtonState.Pressed;
                case MouseButton.XButton2:
                    return m_MouseState.XButton2 == ButtonState.Pressed;
                default:
                    return false;
            }
        }

        InputEventKeyboard LastKeyPressEvent
        {
            get
            {
                for (int i = m_EventsNext.Count; i >= 0; i--)
                {
                    if ((m_EventsNext[i - 1] as InputEventKeyboard)?.EventType == KeyboardEvent.Press)
                    {
                        return m_EventsNext[i - 1] as InputEventKeyboard;
                    }
                }
                return null;
            }
        }

        public List<InputEventKeyboard> GetKeyboardEvents()
        {
            List<InputEventKeyboard> list = new List<InputEventKeyboard>();
            foreach (InputEvent e in m_Events)
            {
                if (!e.Handled && e is InputEventKeyboard)
                {
                    list.Add(e as InputEventKeyboard);
                }
            }
            return list;
        }

        public List<InputEventMouse> GetMouseEvents()
        {
            List<InputEventMouse> list = new List<InputEventMouse>();
            foreach (InputEvent e in m_Events)
            {
                if (!e.Handled && e is InputEventMouse)
                {
                    list.Add(e as InputEventMouse);
                }
            }
            return list;
        }

        public bool HandleKeyboardEvent(KeyboardEvent type, WinKeys key, bool shift, bool alt, bool ctrl)
        {
            foreach (InputEvent e in m_Events)
            {
                if (!e.Handled && e is InputEventKeyboard)
                {
                    InputEventKeyboard ek = e as InputEventKeyboard;
                    if (ek.EventType == type && ek.KeyCode == key && 
                        ek.Shift == shift && ek.Alt == alt && ek.Control == ctrl)
                    {
                        e.Handled = true;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HandleMouseEvent(MouseEvent type, MouseButton mb)
        {
            foreach (InputEvent e in m_Events)
            {
                if (!e.Handled && e is InputEventMouse)
                {
                    InputEventMouse em = (InputEventMouse)e;
                    if (em.EventType == type && em.Button == mb)
                    {
                        e.Handled = true;
                        return true;
                    }
                }
            }
            return false;
        }

        void OnMouseDown(InputEventMouse e)
        {
            m_LastMouseDown = e;
            m_LastMouseDownTime = m_TheTime;
            AddEvent(m_LastMouseDown);
        }

        void OnMouseUp(InputEventMouse e)
        {
            if (m_MouseIsDragging)
            {
                AddEvent(new InputEventMouse(MouseEvent.DragEnd, e));
                m_MouseIsDragging = false;
            }
            else
            {
                if (m_LastMouseDown != null)
                {
                    if (!DistanceBetweenPoints(m_LastMouseDown.Position, e.Position, MouseClickMaxDelta))
                    {
                        AddEvent(new InputEventMouse(MouseEvent.Click, e));

                        if ((m_TheTime - m_LastMouseClickTime <= Settings.UserInterface.Mouse.DoubleClickMS) &&
                           !DistanceBetweenPoints(m_LastMouseClick.Position, e.Position, MouseClickMaxDelta))
                        {
                            m_LastMouseClickTime = 0f;
                            AddEvent(new InputEventMouse(MouseEvent.DoubleClick, e));
                        }
                        else
                        {
                            m_LastMouseClickTime = m_TheTime;
                            m_LastMouseClick = e;
                        }
                    }
                }
            }
            AddEvent(new InputEventMouse(MouseEvent.Up, e));
            m_LastMouseDown = null;
        }

        void OnMouseMove(InputEventMouse e)
        {
            AddEvent(new InputEventMouse(MouseEvent.Move, e));
            if (!m_MouseIsDragging && m_LastMouseDown != null)
            {
                if (DistanceBetweenPoints(m_LastMouseDown.Position, e.Position, MouseDragBeginDistance))
                {
                    AddEvent(new InputEventMouse(MouseEvent.DragBegin, e));
                    m_MouseIsDragging = true;
                }
            }
        }

        void OnKeyDown(InputEventKeyboard e)
        {
            if (e.DataPreviousState == 0)
            {
                AddEvent(new InputEventKeyboard(KeyboardEvent.Down, e));
            }
            for (int i = 0; i < e.DataRepeatCount; i++)
            {
                AddEvent(new InputEventKeyboard(KeyboardEvent.Press, e));
            }
        }

        void OnKeyUp(InputEventKeyboard e)
        {
            AddEvent(new InputEventKeyboard(KeyboardEvent.Up, e));
        }

        void OnKeyChar(InputEventKeyboard e)
        {
            // Control key sends a strange wm_char message ...
            if (e.Control && !e.Alt)
            {
                return;
            }
            InputEventKeyboard ek = LastKeyPressEvent;
            if (ek == null)
            {
                Tracer.Warn("No corresponding KeyPress event for a WM_CHAR message.");
            }
            else
            {
                ek.OverrideKeyChar(e.KeyCode);
            }
        }

        void AddEvent(InputEvent e)
        {
            m_EventsNext.Add(e);
        }

        bool DistanceBetweenPoints(Point initial, Point final, int distance)
        {
            if (Math.Abs(final.X - initial.X) + Math.Abs(final.Y - initial.Y) > distance)
            {
                return true;
            }
            return false;
        }
    }
}