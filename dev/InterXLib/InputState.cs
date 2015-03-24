using InterXLib.Input.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Forms;

namespace InterXLib
{
    public class InputState
    {
        // Base WndProc
        WndProc m_WndProc;

        // Input states
        bool m_IsInitialized = false;
        MouseState m_MouseStateThisFrame;
        MouseState m_MouseStateLastFrame;

        // Event lists.
        List<InputEvent> m_EventsThisFrame = new List<InputEvent>();
        List<InputEvent> m_EventsAccumulating = new List<InputEvent>();
        List<InputEvent> m_EventsAccumulatingAlternate = new List<InputEvent>();
        bool m_EventsAccumulatingUseAlternate = false;

        // Mouse dragging support
        bool m_MouseIsDragging = false;
        InputEventMouse m_LastMouseDown = null;
        InputEventMouse m_LastMouseClick = null;
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

        public bool IsCtrlDown
        {
            get;
            private set;
        }

        public bool IsAltDown
        {
            get;
            private set;
        }

        public List<InputEventKeyboard> GetKeyboardEvents()
        {
            List<InputEventKeyboard> list = new List<InputEventKeyboard>();
            foreach (InputEvent e in m_EventsThisFrame)
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

        public void Update(double totalTime, double frameTime)
        {
            m_TheTime = (float)totalTime;

            if (!m_IsInitialized)
            {
                m_MouseStateLastFrame = m_MouseStateThisFrame = m_WndProc.MouseState;
                m_IsInitialized = true;
            }

            m_MouseStateLastFrame = m_MouseStateThisFrame;
            m_MouseStateThisFrame = CreateMouseState(m_WndProc.MouseState);

            // update mouse stationary business
            if (hasMouseBeenStationarySinceLastUpdate)
                m_mouseStationaryMS += (float)frameTime;
            else
                m_mouseStationaryMS = 0;
            
            copyEvents();
        }

        public MouseState CreateMouseState(MouseState state)
        {
            MouseState newstate = new MouseState((int)(state.X / Settings.ScreenDPI.X), (int)(state.Y / Settings.ScreenDPI.Y),
                state.ScrollWheelValue, state.LeftButton, state.MiddleButton, state.RightButton, state.XButton1, state.XButton2);
            return newstate;
        }

        private float m_TheTime = -1f;

        private float m_mouseStationaryMS = 0f;
        public int MouseStationaryTimeMS
        {
            get
            {
                return (int)m_mouseStationaryMS;
            }
        }

        public Point MousePosition
        {
            get
            {
                Point p = new Point();
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

        public bool HandleKeyboardEvent(KeyboardEventType type, WinKeys key, bool shift, bool alt, bool ctrl)
        {
            foreach (InputEvent e in m_EventsThisFrame)
            {
                if (!e.Handled && e is InputEventKeyboard)
                {
                    InputEventKeyboard ek = (InputEventKeyboard)e;
                    if (ek.EventType == type &&
                        ek.KeyCode == key &&
                        ek.Shift == shift &&
                        ek.Alt == alt &&
                        ek.Control == ctrl)
                    {
                        e.Handled = true;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HandleMouseEvent(MouseEvent type, InterXLib.Input.Windows.MouseButton mb)
        {
            foreach (InputEvent e in m_EventsThisFrame)
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

        private void onMouseWheel(InputEventMouse e)
        {
            addEvent(e);
        }

        private void onMouseDown(InputEventMouse e)
        {
            m_LastMouseDown = e;
            m_LastMouseDownTime = m_TheTime;
            addEvent(m_LastMouseDown);
        }

        private void onMouseUp(InputEventMouse e)
        {
            if (m_MouseIsDragging)
            {
                addEvent(new InputEventMouse(MouseEvent.DragEnd, e));
                m_MouseIsDragging = false;
            }
            else
            {
                if (!DistanceBetweenPoints(m_LastMouseDown.Position, e.Position, MouseClickMaxDelta))
                {
                    addEvent(new InputEventMouse(MouseEvent.Click, e));
                    
                    if ((m_TheTime - m_LastMouseClickTime <= Settings.SecondsForDoubleClick * 1000d) &&
                        !DistanceBetweenPoints(m_LastMouseClick.Position, e.Position, MouseClickMaxDelta))
                    {
                        m_LastMouseClickTime = 0f;
                        addEvent(new InputEventMouse(MouseEvent.DoubleClick, e));
                    }
                    else
                    {
                        m_LastMouseClickTime = m_TheTime;
                        m_LastMouseClick = e;
                    }
                }
            }
            addEvent(new InputEventMouse(MouseEvent.Up, e));
            m_LastMouseDown = null;
        }

        private void onMouseMove(InputEventMouse e)
        {
            addEvent(new InputEventMouse(MouseEvent.Move, e));
            if (!m_MouseIsDragging && m_LastMouseDown != null)
            {
                if (DistanceBetweenPoints(m_LastMouseDown.Position, e.Position, MouseDragBeginDistance))
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
                addEvent(new InputEventKeyboard(KeyboardEventType.Down, e));
            }
            // handle the key presses. Possibly multiple per keydown message.
            for (int i = 0; i < e.Data_RepeatCount; i++)
            {
                addEvent(new InputEventKeyboard(KeyboardEventType.Press, e));
            }
        }

        private void onKeyUp(InputEventKeyboard e)
        {
            addEvent(new InputEventKeyboard(KeyboardEventType.Up, e));
        }

        private void onKeyChar(InputEventKeyboard e)
        {
            // Control key sends a strange wm_char message ...
            if (e.Control && !e.Alt)
                return;

            InputEventKeyboard pressEvent = LastKeyPressEvent;
            if (pressEvent == null)
                Logging.Fatal("No corresponding KeyPress event for this WM_CHAR message. Please report this error to poplicola@ultimaxna.com");
            else
            {
                pressEvent.OverrideKeyChar(e.KeyCode);
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
            IsAltDown = e.Alt;
            IsCtrlDown = e.Control;
        }

        private InputEventKeyboard LastKeyPressEvent
        {
            get
            {
                List<InputEvent> list = (m_EventsAccumulatingUseAlternate) ? m_EventsAccumulatingAlternate : m_EventsAccumulating;
                for (int i = list.Count; i > 0; i--)
                {
                    InputEvent e = list[i - 1];
                    if ((e is InputEventKeyboard) && (((InputEventKeyboard)e).EventType == KeyboardEventType.Press))
                    {
                        return (InputEventKeyboard)e;
                    }
                }
                return null;
            }
        }

        private bool DistanceBetweenPoints(Point initial, Point final, int distance)
        {
            if (Math.Abs(final.X - initial.X) + Math.Abs(final.Y - initial.Y) > distance)
                return true;
            else
                return false;
        }

        /*
         * These routines are disabled because they could be used by a programmer to get the state of input without handling the
         * InputEvent that created that state, thus allowing two or more parts of the program to respond to the save event. This
         * could be bad!
         * 
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
        */
    }
}
