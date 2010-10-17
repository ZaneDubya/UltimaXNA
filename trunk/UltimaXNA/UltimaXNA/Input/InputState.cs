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
        MouseState _mouseStateThisFrame;
        KeyboardState _keyboardStateThisFrame;

        MouseState _mouseStateLastFrame;
        KeyboardState _keyboardStateLastFrame;

        List<InputEvent> _eventsThisFrame = new List<InputEvent>();
        List<InputEvent> _eventsAccumulating = new List<InputEvent>();
        List<InputEvent> _eventsAccumulatingAlternate = new List<InputEvent>();
        bool _eventsAccumulatingUseAlternate = false;

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
            foreach (InputEventKeyboard e in _eventsThisFrame)
            {
                if (e is InputEventKeyboard)
                    list.Add(e);
            }
            return list;
        }

        public List<InputEventMouse> GetMouseEvents()
        {
            List<InputEventMouse> list = new List<InputEventMouse>();
            foreach (InputEventMouse e in _eventsThisFrame)
            {
                if (e is InputEventMouse)
                    list.Add(e);
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
            if (_eventsAccumulatingUseAlternate)
                _eventsAccumulatingAlternate.Add(e);
            else
                _eventsAccumulating.Add(e);
        }

        public Point2D MousePosition
        {
            get
            {
                Point2D p = new Point2D();
                p.x = _mouseStateThisFrame.X;
                p.y = _mouseStateThisFrame.Y;
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

        public bool IsMouseButtonDown(MouseButton button)
        {
            if ((getMouseButtons(_mouseStateThisFrame) & button) == button)
                return true;
            else
                return false;
        }

        public bool IsMouseButtonUp(MouseButton button)
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

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            
        }
    }
}
