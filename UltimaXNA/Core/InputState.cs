using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.Interface.Input;

namespace UltimaXNA.Interface
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
        InputEventM m_LastMouseDown = null;
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

        public List<InputEventKB> GetKeyboardEvents()
        {
            List<InputEventKB> list = new List<InputEventKB>();
            foreach (InputEvent e in m_EventsThisFrame  )
            {
                if (!e.Handled && e is InputEventKB)
                    list.Add((InputEventKB)e);
            }
            return list;
        }

        public List<InputEventM> GetMouseEvents()
        {
            List<InputEventM> list = new List<InputEventM>();
            foreach (InputEvent e in m_EventsThisFrame)
            {
                if (!e.Handled && e is InputEventM)
                    list.Add((InputEventM)e);
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


        public bool HandleKeyboardEvent(KeyboardEvent type, WinKeys key, bool shift, bool alt, bool ctrl)
        {
            List<InputEventKB> events = GetKeyboardEvents();
            foreach (InputEventKB e in events)
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
            List<InputEventM> events = GetMouseEvents();
            foreach (InputEventM e in events)
            {
                if (e.EventType == type && e.Button == mb)
                {
                    e.Handled = true;
                    return true;
                }
            }
            return false;
        }

        private void onMouseWheel(EventArgsMouse e)
        {
            addEvent(new InputEventM(MouseEvent.WheelScroll, e));
        }

        private void onMouseDown(EventArgsMouse e)
        {
            m_LastMouseDown = new InputEventM(MouseEvent.Down, e);
            m_LastMouseDownTime = UltimaVars.EngineVars.TheTime;
            addEvent(m_LastMouseDown);
        }

        private void onMouseUp(EventArgsMouse e)
        {
            addEvent(new InputEventM(MouseEvent.Up, e));
            if (m_MouseIsDragging)
            {
                addEvent(new InputEventM(MouseEvent.DragEnd, e));
                m_MouseIsDragging = false;
            }
            else
            {
                if (!Utility.IsPointThisDistanceAway(m_LastMouseDown.Position, e.Position, MouseClickMaxDelta))
                {
                    if ((UltimaVars.EngineVars.TheTime - m_LastMouseClickTime <= UltimaVars.EngineVars.SecondsForDoubleClick))
                    {
                        m_LastMouseClickTime = 0f;
                        addEvent(new InputEventM(MouseEvent.DoubleClick, e));
                    }
                    else
                    {
                        m_LastMouseClickTime = UltimaVars.EngineVars.TheTime;
                        addEvent(new InputEventM(MouseEvent.Click, e));
                    }
                }
            }
            m_LastMouseDown = null;
        }

        private void onMouseMove(EventArgsMouse e)
        {
            addEvent(new InputEventM(MouseEvent.Move, e));
            if (!m_MouseIsDragging && m_LastMouseDown != null)
            {
                if (Utility.IsPointThisDistanceAway(m_LastMouseDown.Position, e.Position, MouseDragBeginDistance))
                {
                    addEvent(new InputEventM(MouseEvent.DragBegin, e));
                    m_MouseIsDragging = true;
                }
            }
        }

        private void onKeyDown(EventArgsKeyboard e)
        {
            // handle the initial key down
            if (e.Data_PreviousState == 0)
            {
                addEvent(new InputEventKB(KeyboardEvent.Down, e));
            }
            // handle the key presses. Possibly multiple per keydown message.
            for (int i = 0; i < e.Data_RepeatCount; i++)
            {
                addEvent(new InputEventKB(KeyboardEvent.Press, e));
            }
        }

        private void onKeyUp(EventArgsKeyboard e)
        {
            addEvent(new InputEventKB(KeyboardEvent.Up, e));
        }

        private void onKeyChar(EventArgsKeyboard e)
        {
            // Control key sends a strange wm_char message ...
            if (e.Control && !e.Alt)
                return;

            InputEventKB pressEvent = LastKeyPressEvent;
            if (pressEvent == null)
                throw new Exception("No corresponding KeyPress event for this WM_CHAR message. Please report this error to poplicola@ultimaxna.com");
            else
            {
                pressEvent.OverrideKeyChar(e.KeyCode);
                if (UltimaVars.DebugVars.Flag_LogKeyboardChars)
                {
                    Diagnostics.Logger.Debug("Char: " + pressEvent.KeyChar);
                }
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

        private InputEventKB LastKeyPressEvent
        {
            get
            {
                List<InputEvent> list = (m_EventsAccumulatingUseAlternate) ? m_EventsAccumulatingAlternate : m_EventsAccumulating;
                for (int i = list.Count; i > 0; i--)
                {
                    InputEvent e = list[i - 1];
                    if ((e is InputEventKB) && (((InputEventKB)e).EventType == KeyboardEvent.Press))
                    {
                        return (InputEventKB)e;
                    }
                }
                return null;
            }
        }
    }
}
