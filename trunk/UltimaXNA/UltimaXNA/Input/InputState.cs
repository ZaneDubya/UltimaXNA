using System;
using System.Collections.Generic;
using System.Diagnostics;
using UltimaXNA.Input;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.Input.Events;

namespace UltimaXNA.Input
{

    public class InputState : WndProc, IInputState, IGameComponent
    {
        // Input states
        MouseState _mouseStateThisFrame;
        KeyboardState _keyboardStateThisFrame;
        MouseState _mouseStateLastFrame;
        KeyboardState _keyboardStateLastFrame;

        // Event lists.
        List<InputEvent> _eventsThisFrame = new List<InputEvent>();
        List<InputEvent> _eventsAccumulating = new List<InputEvent>();
        List<InputEvent> _eventsAccumulatingAlternate = new List<InputEvent>();
        bool _eventsAccumulatingUseAlternate = false;

        // two variables used for mouse dragging support
        bool _mouseIsDragging = false;
        InputEventMouse _lastMouseDown = null;

        // Debug logging
        public static Diagnostics.Logger _log = new Diagnostics.Logger("InputState");

        public InputState(Game game)
            : base(game.Window.Handle)
        {

        }

        public void Initialize()
        {
            _keyboardStateLastFrame = _keyboardStateThisFrame = getKeyState();
            _mouseStateLastFrame = _mouseStateThisFrame = getMouseState();
        }

        public List<InputEventKeyboard> GetKeyboardEvents()
        {
            List<InputEventKeyboard> list = new List<InputEventKeyboard>();
            foreach (InputEvent e in _eventsThisFrame  )
            {
                if (!e.Handled && e is InputEventKeyboard)
                    list.Add((InputEventKeyboard)e);
            }
            return list;
        }

        public List<InputEventMouse> GetMouseEvents()
        {
            List<InputEventMouse> list = new List<InputEventMouse>();
            foreach (InputEvent e in _eventsThisFrame)
            {
                if (!e.Handled && e is InputEventMouse)
                    list.Add((InputEventMouse)e);
            }
            return list;
        }

        public void Update(GameTime gameTime)
        {
            _mouseStateLastFrame = _mouseStateThisFrame;
            _mouseStateThisFrame = getMouseState();

            _keyboardStateLastFrame = _keyboardStateThisFrame;
            _keyboardStateThisFrame = getKeyState();

            copyEvents();
        }

        private void copyEvents()
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

        protected void addEvent(InputEvent e)
        {
            List<InputEvent> list = (_eventsAccumulatingUseAlternate) ? _eventsAccumulatingAlternate : _eventsAccumulating;
            list.Add(e);
        }

        protected InputEventKeyboard getLastKeyPressEvent()
        {
            List<InputEvent> list = (_eventsAccumulatingUseAlternate) ? _eventsAccumulatingAlternate : _eventsAccumulating;
            for (int i = list.Count; i > 0; i--)
            {
                InputEvent e = list[i - 1];
                if ((e is InputEventKeyboard) && (((InputEventKeyboard)e).EventType == KeyboardEvent.Press))
                {
                    return (InputEventKeyboard)e;
                }
            }
            return null;
        }

        public Point2D MousePosition
        {
            get
            {
                Point2D p = new Point2D();
                p.X = _mouseStateThisFrame.X;
                p.Y = _mouseStateThisFrame.Y;
                return p;
            }
        }

        public bool IsMouseStationarySinceLastUpdate()
        {
            if ((_mouseStateLastFrame.X == _mouseStateThisFrame.X) &&
                (_mouseStateLastFrame.Y == _mouseStateThisFrame.Y))
                return true;
            return false;
        }

        public bool IsMouseButtonDown(MouseButtonInternal button)
        {
            if ((getMouseButtons(_mouseStateThisFrame) & button) == button)
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

        public bool IsKeyUp(WinKeys key)
        {
            if (IsKeyDown(key))
                return false;
            else
                return true;
        }


        public bool HandleKeyboardEvent(KeyboardEvent type, WinKeys key, bool shift, bool alt, bool ctrl)
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
                if (e.EventType == type)
                {
                    e.Handled = true;
                    return true;
                }
            }
            return false;
        }

        protected override void OnMouseWheel(EventArgsMouse e)
        {
            addEvent(new InputEventMouse(MouseEvent.WheelScroll, e));
        }

        protected override void OnMouseDown(EventArgsMouse e)
        {
            _lastMouseDown = new InputEventMouse(MouseEvent.Down, e);
            addEvent(_lastMouseDown);
        }

        protected override void OnMouseUp(EventArgsMouse e)
        {
            addEvent(new InputEventMouse(MouseEvent.Up, e));
            if (_mouseIsDragging)
            {
                addEvent(new InputEventMouse(MouseEvent.DragEnd, e));
                _mouseIsDragging = false;
            }
            else
            {
                addEvent(new InputEventMouse(MouseEvent.Click, e));
            }
        }

        protected override void OnMouseMove(EventArgsMouse e)
        {
            addEvent(new InputEventMouse(MouseEvent.Move, e));
            if (!_mouseIsDragging)
            {

            }
        }

        protected override void OnKeyDown(EventArgsKeyboard e)
        {
            // handle the initial key down
            if (e.Data_PreviousState == 0)
            {
                addEvent(new InputEventKeyboard(KeyboardEvent.Down, e));
            }
            // handle the key presses. Possibly multiple per keydown message.
            for (int i = 0; i < e.Data_RepeatCount; i++)
            {
                addEvent(new InputEventKeyboard(KeyboardEvent.Press, e));
            }
        }

        protected override void OnKeyUp(EventArgsKeyboard e)
        {
            addEvent(new InputEventKeyboard(KeyboardEvent.Up, e));
        }

        protected override void OnChar(EventArgsKeyboard e)
        {
            InputEventKeyboard pressEvent = getLastKeyPressEvent();
            if (pressEvent == null)
                throw new Exception("No corresponding KeyPress event for this WM_CHAR message. Please report this error to poplicola@ultimaxna.com");
            else
            {
                pressEvent.OverrideKeyChar(e.KeyCode);
                _log.Debug("Char: " + pressEvent.KeyChar);
            }
        }
    }
}
