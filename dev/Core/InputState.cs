using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using InterXLib.Input.Windows;

namespace UltimaXNA
{
    public class InputState
    {
        // Base WndProc
        WndProc m_WndProc;

        // Input states
        bool m_IsInitialized = false;
        MouseState m_MouseStateThisFrame;
        KeyboardState m_KeyboardStateThisFrame;
        MouseState m_MouseStateLastFrame;
        KeyboardState m_KeyboardStateLastFrame;

        // Event lists.
        List<InputEvent> m_EventsThisFrame = new List<InputEvent>();
        List<InputEvent> m_EventsAccumulating = new List<InputEvent>();
        List<InputEvent> m_EventsAccumulatingAlternate = new List<InputEvent>();
        bool m_EventsAccumulatingUseAlternate = false;

        // Mouse dragging support
        bool m_MouseIsDragging = false;
        InputEventMouse m_LastMouseDown = null;
        float m_LastMouseDownTime = 0f;
        float m_LastMouseClickTime = 0f;
        private const int MouseDragBeginDistance = 2;
        private const int MouseClickMaxDelta = 2;

        public void Initialize(IntPtr handle)
        {
            m_WndProc = new WndProc(handle);
            m_WndProc.MouseWheel += onMouseWheel;
            m_WndProc.MouseMove += onMouseMove;
            m_WndProc.MouseUp += onMouseUp;
            m_WndProc.MouseDown += onMouseDown;
            m_WndProc.KeyDown += onKeyDown;
            m_WndProc.KeyUp += onKeyUp;
            m_WndProc.KeyChar += onKeyChar;
        }

        public List<InputEventKeyboard> GetKeyboardEvents()
        {
            List<InputEventKeyboard> list = new List<InputEventKeyboard>();
            foreach (InputEvent e in m_EventsThisFrame  )
            {
                if (!e.Handled && e is InputEventKeyboard)
                    list.Add((InputEventKeyboard)e);
            }
            return list;
        }

        public List<InputEventMouse> GetMouseEvents()
        {
            List<InputEventMouse> list = new List<InputEventMouse>();
            foreach (InputEvent e in m_EventsThisFrame)
            {
                if (!e.Handled && e is InputEventMouse)
                    list.Add((InputEventMouse)e);
            }
            return list;
        }

        public void Update(GameTime gameTime)
        {
            if (!m_IsInitialized)
            {
                m_KeyboardStateLastFrame = m_KeyboardStateThisFrame = m_WndProc.KeyboardState;
                m_MouseStateLastFrame = m_MouseStateThisFrame = m_WndProc.MouseState;
            }

            if (UltimaVars.EngineVars.MouseEnabled)
            {
                m_MouseStateLastFrame = m_MouseStateThisFrame;
                m_MouseStateThisFrame = m_WndProc.MouseState;

                // update mouse stationary business
                if (hasMouseBeenStationarySinceLastUpdate)
                    _mouseStationaryMS += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                else
                    _mouseStationaryMS = 0;
            }

            m_KeyboardStateLastFrame = m_KeyboardStateThisFrame;
            m_KeyboardStateThisFrame = m_WndProc.KeyboardState;
            
            copyEvents();
        }

        private float _mouseStationaryMS = 0f;
        public int MouseStationaryTimeMS
        {
            get
            {
                return (int)_mouseStationaryMS;
            }
        }

        public Point2D MousePosition
        {
            get
            {
                Point2D p = new Point2D();
                p.X = m_MouseStateThisFrame.X;
                p.Y = m_MouseStateThisFrame.Y;
                return p;
            }
        }

        private bool hasMouseBeenStationarySinceLastUpdate
        {
            get
            {
                if ((m_MouseStateLastFrame.X == m_MouseStateThisFrame.X) &&
                    (m_MouseStateLastFrame.Y == m_MouseStateThisFrame.Y))
                    return true;
                return false;
            }
        }

        public bool IsMouseButtonDown(MouseButtonInternal button)
        {
            if ((m_WndProc.MouseButtons(m_MouseStateThisFrame) & button) == button)
                return true;
            else
                return false;
        }

        public bool IsMouseButtonUp(MouseButtonInternal button)
        {
            if (IsMouseButtonUp(button))
                return false;
            else
                return true;
        }

        public bool IsKeyDown(WinKeys key)
        {
            Keys[] pressed = m_KeyboardStateThisFrame.GetPressedKeys();
            foreach (Keys k in pressed)
            {
                if (k == (Keys)key)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsKeyUp(WinKeys key)
        {
            if (IsKeyDown(key))
                return false;
            else
                return true;
        }


        public bool HandleKeyboardEvent(KeyboardEventType type, WinKeys key, bool shift, bool alt, bool ctrl)
        {
            List<InputEventKeyboard> events = GetKeyboardEvents();
            foreach (InputEventKeyboard e in events)
            {
                if (e.EventType == type && 
                    e.KeyCode == key &&
                    e.Shift == shift &&
                    e.Alt == alt &&
                    e.Control == ctrl)
                {
                    e.Handled = true;
                    return true;
                }
            }
            return false;
        }

        public bool HandleMouseEvent(MouseEvent type, MouseButton mb)
        {
            List<InputEventMouse> events = GetMouseEvents();
            foreach (InputEventMouse e in events)
            {
                if (e.EventType == type && e.Button == mb)
                {
                    e.Handled = true;
                    return true;
                }
            }
            return false;
        }

        private void onMouseWheel(InputEventMouse e)
        {
            addEvent(new InputEventMouse(MouseEvent.WheelScroll, e));
        }

        private void onMouseDown(InputEventMouse e)
        {
            m_LastMouseDown = new InputEventMouse(MouseEvent.Down, e);
            m_LastMouseDownTime = UltimaVars.EngineVars.TheTime;
            addEvent(m_LastMouseDown);
        }

        private void onMouseUp(InputEventMouse e)
        {
            addEvent(new InputEventMouse(MouseEvent.Up, e));
            if (m_MouseIsDragging)
            {
                addEvent(new InputEventMouse(MouseEvent.DragEnd, e));
                m_MouseIsDragging = false;
            }
            else
            {
                if (!Utility.IsPointThisDistanceAway(m_LastMouseDown.Position, e.Position, MouseClickMaxDelta))
                {
                    if ((UltimaVars.EngineVars.TheTime - m_LastMouseClickTime <= UltimaVars.EngineVars.SecondsForDoubleClick))
                    {
                        m_LastMouseClickTime = 0f;
                        addEvent(new InputEventMouse(MouseEvent.DoubleClick, e));
                    }
                    else
                    {
                        m_LastMouseClickTime = UltimaVars.EngineVars.TheTime;
                        addEvent(new InputEventMouse(MouseEvent.Click, e));
                    }
                }
            }
            m_LastMouseDown = null;
        }

        private void onMouseMove(InputEventMouse e)
        {
            addEvent(new InputEventMouse(MouseEvent.Move, e));
            if (!m_MouseIsDragging && m_LastMouseDown != null)
            {
                if (Utility.IsPointThisDistanceAway(m_LastMouseDown.Position, e.Position, MouseDragBeginDistance))
                {
                    addEvent(new InputEventMouse(MouseEvent.DragBegin, e));
                    m_MouseIsDragging = true;
                }
            }
        }

        private void onKeyDown(InputEventKeyboard e)
        {
            // handle the initial key down
            if (e.Data_PreviousState == 0)
            {
                addEvent(e);
            }
            // handle the key presses. Possibly multiple per keydown message.
            for (int i = 0; i < e.Data_RepeatCount; i++)
            {
                addEvent(e);
            }
        }

        private void onKeyUp(InputEventKeyboard e)
        {
            addEvent(e);
        }

        private void onKeyChar(InputEventKeyboard e)
        {
            // Control key sends a wm_char message that we should not handle.
            if (e.Control && !e.Alt)
                return;

            e.OverrideKeyChar(e.KeyCode);
            addEvent(e);
            if (UltimaVars.DebugVars.Flag_LogKeyboardChars)
            {
                Diagnostics.Logger.Debug("Char: " + e.KeyChar);
            }
        }

        private void copyEvents()
        {
            // use alternate events list while we copy the accumulated events to the events list for this frame.
            m_EventsAccumulatingUseAlternate = true;

            // clear the old events list, copy all accumulated events to the this frame event list, then clear the accumulated events list.
            m_EventsThisFrame.Clear();
            foreach (InputEvent e in m_EventsAccumulating)
                m_EventsThisFrame.Add(e);
            m_EventsAccumulating.Clear();

            // start accumulating new events in the standard accumulating list again.
            m_EventsAccumulatingUseAlternate = false;

            // copy all events in the alternate accumulating list to the this frame event list, then clear the alternate accumulating list.
            foreach (InputEvent e in m_EventsAccumulatingAlternate)
                m_EventsThisFrame.Add(e);
            m_EventsAccumulatingAlternate.Clear();
        }

        private void addEvent(InputEvent e)
        {
            List<InputEvent> list = (m_EventsAccumulatingUseAlternate) ? m_EventsAccumulatingAlternate : m_EventsAccumulating;
            list.Add(e);
        }
    }
}
