using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.Interface.Input;

namespace UltimaXNA
{

    public class InputState
    {
        // Base WndProc
        static WndProc m_WndProc;

        // Input states
        static bool _isInitialized = false;
        static MouseState _mouseStateThisFrame;
        static KeyboardState _keyboardStateThisFrame;
        static MouseState _mouseStateLastFrame;
        static KeyboardState _keyboardStateLastFrame;

        // Event lists.
        static List<InputEvent> _eventsThisFrame = new List<InputEvent>();
        static List<InputEvent> _eventsAccumulating = new List<InputEvent>();
        static List<InputEvent> _eventsAccumulatingAlternate = new List<InputEvent>();
        static bool _eventsAccumulatingUseAlternate = false;

        // Mouse dragging support
        static bool _mouseIsDragging = false;
        static InputEventM _lastMouseDown = null;
        static float _lastMouseDownTime = 0f;
        static float _lastMouseClickTime = 0f;
        const int MouseDragBeginDistance = 2;
        const int MouseClickMaxDelta = 2;

        public static void Initialize(IntPtr handle)
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

        public static List<InputEventKB> GetKeyboardEvents()
        {
            List<InputEventKB> list = new List<InputEventKB>();
            foreach (InputEvent e in _eventsThisFrame  )
            {
                if (!e.Handled && e is InputEventKB)
                    list.Add((InputEventKB)e);
            }
            return list;
        }

        public static List<InputEventM> GetMouseEvents()
        {
            List<InputEventM> list = new List<InputEventM>();
            foreach (InputEvent e in _eventsThisFrame)
            {
                if (!e.Handled && e is InputEventM)
                    list.Add((InputEventM)e);
            }
            return list;
        }

        public static void Update(GameTime gameTime)
        {
            if (!_isInitialized)
            {
                _keyboardStateLastFrame = _keyboardStateThisFrame = m_WndProc.KeyboardState;
                _mouseStateLastFrame = _mouseStateThisFrame = m_WndProc.MouseState;
            }

            if (ClientVars.EngineVars.MouseEnabled)
            {
                _mouseStateLastFrame = _mouseStateThisFrame;
                _mouseStateThisFrame = m_WndProc.MouseState;

                // update mouse stationary business
                if (hasMouseBeenStationarySinceLastUpdate)
                    _mouseStationaryMS += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                else
                    _mouseStationaryMS = 0;
            }

            _keyboardStateLastFrame = _keyboardStateThisFrame;
            _keyboardStateThisFrame = m_WndProc.KeyboardState;
            
            copyEvents();
        }

        private static float _mouseStationaryMS = 0f;
        public static int MouseStationaryTimeMS
        {
            get
            {
                return (int)_mouseStationaryMS;
            }
        }

        public static Point2D MousePosition
        {
            get
            {
                Point2D p = new Point2D();
                p.X = _mouseStateThisFrame.X;
                p.Y = _mouseStateThisFrame.Y;
                return p;
            }
        }

        private static bool hasMouseBeenStationarySinceLastUpdate
        {
            get
            {
                if ((_mouseStateLastFrame.X == _mouseStateThisFrame.X) &&
                    (_mouseStateLastFrame.Y == _mouseStateThisFrame.Y))
                    return true;
                return false;
            }
        }

        public static bool IsMouseButtonDown(MouseButtonInternal button)
        {
            if ((m_WndProc.MouseButtons(_mouseStateThisFrame) & button) == button)
                return true;
            else
                return false;
        }

        public static bool IsMouseButtonUp(MouseButtonInternal button)
        {
            if (IsMouseButtonUp(button))
                return false;
            else
                return true;
        }

        public static bool IsKeyDown(WinKeys key)
        {
            Keys[] pressed = _keyboardStateThisFrame.GetPressedKeys();
            foreach (Keys k in pressed)
            {
                if (k == (Keys)key)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsKeyUp(WinKeys key)
        {
            if (IsKeyDown(key))
                return false;
            else
                return true;
        }


        public static bool HandleKeyboardEvent(KeyboardEvent type, WinKeys key, bool shift, bool alt, bool ctrl)
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

        public static bool HandleMouseEvent(MouseEvent type, MouseButton mb)
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

        private static void onMouseWheel(EventArgsMouse e)
        {
            addEvent(new InputEventM(MouseEvent.WheelScroll, e));
        }

        private static void onMouseDown(EventArgsMouse e)
        {
            _lastMouseDown = new InputEventM(MouseEvent.Down, e);
            _lastMouseDownTime = ClientVars.EngineVars.TheTime;
            addEvent(_lastMouseDown);
        }

        private static void onMouseUp(EventArgsMouse e)
        {
            addEvent(new InputEventM(MouseEvent.Up, e));
            if (_mouseIsDragging)
            {
                addEvent(new InputEventM(MouseEvent.DragEnd, e));
                _mouseIsDragging = false;
            }
            else
            {
                if (!Utility.IsPointThisDistanceAway(_lastMouseDown.Position, e.Position, MouseClickMaxDelta))
                {
                    if ((ClientVars.EngineVars.TheTime - _lastMouseClickTime <= ClientVars.EngineVars.SecondsForDoubleClick))
                    {
                        _lastMouseClickTime = 0f;
                        addEvent(new InputEventM(MouseEvent.DoubleClick, e));
                    }
                    else
                    {
                        _lastMouseClickTime = ClientVars.EngineVars.TheTime;
                        addEvent(new InputEventM(MouseEvent.Click, e));
                    }
                }
            }
            _lastMouseDown = null;
        }

        private static void onMouseMove(EventArgsMouse e)
        {
            addEvent(new InputEventM(MouseEvent.Move, e));
            if (!_mouseIsDragging && _lastMouseDown != null)
            {
                if (Utility.IsPointThisDistanceAway(_lastMouseDown.Position, e.Position, MouseDragBeginDistance))
                {
                    addEvent(new InputEventM(MouseEvent.DragBegin, e));
                    _mouseIsDragging = true;
                }
            }
        }

        private static void onKeyDown(EventArgsKeyboard e)
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

        private static void onKeyUp(EventArgsKeyboard e)
        {
            addEvent(new InputEventKB(KeyboardEvent.Up, e));
        }

        private static void onKeyChar(EventArgsKeyboard e)
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
                if (ClientVars.DebugVars.Flag_LogKeyboardChars)
                {
                    Diagnostics.Logger.Debug("Char: " + pressEvent.KeyChar);
                }
            }
        }

        private static void copyEvents()
        {
            // use alternate events list while we copy the accumulated events to the events list for this frame.
            _eventsAccumulatingUseAlternate = true;

            // clear the old events list, copy all accumulated events to the this frame event list, then clear the accumulated events list.
            _eventsThisFrame.Clear();
            foreach (InputEvent e in _eventsAccumulating)
                _eventsThisFrame.Add(e);
            _eventsAccumulating.Clear();

            // start accumulating new events in the standard accumulating list again.
            _eventsAccumulatingUseAlternate = false;

            // copy all events in the alternate accumulating list to the this frame event list, then clear the alternate accumulating list.
            foreach (InputEvent e in _eventsAccumulatingAlternate)
                _eventsThisFrame.Add(e);
            _eventsAccumulatingAlternate.Clear();
        }

        private static void addEvent(InputEvent e)
        {
            List<InputEvent> list = (_eventsAccumulatingUseAlternate) ? _eventsAccumulatingAlternate : _eventsAccumulating;
            list.Add(e);
        }

        private static InputEventKB LastKeyPressEvent
        {
            get
            {
                List<InputEvent> list = (_eventsAccumulatingUseAlternate) ? _eventsAccumulatingAlternate : _eventsAccumulating;
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
