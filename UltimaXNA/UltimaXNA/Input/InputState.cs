using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.Input.Core;

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

        // Mouse dragging support
        bool _mouseIsDragging = false;
        InputEventM _lastMouseDown = null;
        float _lastMouseDownTime = 0f;
        float _lastMouseClickTime = 0f;
        const int MouseDragBeginDistance = 2;
        const int MouseClickMaxDelta = 2;

        public InputState(Game game)
            : base(game.Window.Handle)
        {

        }

        public void Initialize()
        {
            _keyboardStateLastFrame = _keyboardStateThisFrame = getKeyState();
            _mouseStateLastFrame = _mouseStateThisFrame = getMouseState();
        }

        public List<InputEventKB> GetKeyboardEvents()
        {
            List<InputEventKB> list = new List<InputEventKB>();
            foreach (InputEvent e in _eventsThisFrame  )
            {
                if (!e.Handled && e is InputEventKB)
                    list.Add((InputEventKB)e);
            }
            return list;
        }

        public List<InputEventM> GetMouseEvents()
        {
            List<InputEventM> list = new List<InputEventM>();
            foreach (InputEvent e in _eventsThisFrame)
            {
                if (!e.Handled && e is InputEventM)
                    list.Add((InputEventM)e);
            }
            return list;
        }

        public void Update(GameTime gameTime)
        {
            _mouseStateLastFrame = _mouseStateThisFrame;
            _mouseStateThisFrame = getMouseState();

            _keyboardStateLastFrame = _keyboardStateThisFrame;
            _keyboardStateThisFrame = getKeyState();

            // update mouse stationary business
            if (isMouseStationarySinceLastUpdate)
                _mouseStationaryMS += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            else
                _mouseStationaryMS = 0;
            
            copyEvents();
        }

        private float _mouseStationaryMS = 0f;
        public int MouseStationaryMS
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
                p.X = _mouseStateThisFrame.X;
                p.Y = _mouseStateThisFrame.Y;
                return p;
            }
        }

        protected bool isMouseStationarySinceLastUpdate
        {
            get
            {
                if ((_mouseStateLastFrame.X == _mouseStateThisFrame.X) &&
                    (_mouseStateLastFrame.Y == _mouseStateThisFrame.Y))
                    return true;
                return false;
            }
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
            addEvent(new InputEventM(MouseEvent.WheelScroll, e));
        }

        protected override void OnMouseDown(EventArgsMouse e)
        {
            _lastMouseDown = new InputEventM(MouseEvent.Down, e);
            _lastMouseDownTime = ClientVars.TheTime;
            addEvent(_lastMouseDown);
        }

        protected override void OnMouseUp(EventArgsMouse e)
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
                    if ((ClientVars.TheTime - _lastMouseClickTime <= ClientVars.SecondsForDoubleClick))
                    {
                        _lastMouseClickTime = 0f;
                        addEvent(new InputEventM(MouseEvent.DoubleClick, e));
                    }
                    else
                    {
                        _lastMouseClickTime = ClientVars.TheTime;
                        addEvent(new InputEventM(MouseEvent.Click, e));
                    }
                }
            }
            _lastMouseDown = null;
        }

        protected override void OnMouseMove(EventArgsMouse e)
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

        protected override void OnKeyDown(EventArgsKeyboard e)
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

        protected override void OnKeyUp(EventArgsKeyboard e)
        {
            addEvent(new InputEventKB(KeyboardEvent.Up, e));
        }

        protected override void OnChar(EventArgsKeyboard e)
        {
            // Control key sends a strange wm_char message ...
            if (e.Control && !e.Alt)
                return;

            InputEventKB pressEvent = getLastKeyPressEvent();
            if (pressEvent == null)
                throw new Exception("No corresponding KeyPress event for this WM_CHAR message. Please report this error to poplicola@ultimaxna.com");
            else
            {
                pressEvent.OverrideKeyChar(e.KeyCode);
                if (ClientVars.DEBUG_LogKeyboardChars)
                {
                    Diagnostics.Logger.Debug("Char: " + pressEvent.KeyChar);
                }
            }
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

        protected InputEventKB getLastKeyPressEvent()
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
