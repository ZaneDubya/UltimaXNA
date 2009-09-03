using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace UltimaXNA.Input
{
    public class MouseEventArgs : EventArgs
    {
        bool _handled = false;

        readonly Vector2 _movementDelta;
        readonly Vector2 _position;

        readonly MouseButtons[] _buttonsDown;
        readonly MouseButtons[] _buttonsUp;
        readonly MouseButtons[] _buttonsPressed;
        readonly MouseButtons[] _buttonsReleased;

        /// <summary>
        /// Gets an array of buttons that were down during the frame the the event was called
        /// </summary>
        public MouseButtons[] ButtonsDown
        {
            get { return _buttonsDown; }
        }

        /// <summary>
        /// Gets an array of buttons that were up during the frame the the event was called
        /// </summary>
        public MouseButtons[] ButtonsUp
        {
            get { return _buttonsUp; }
        }

        /// <summary>
        /// Gets an array of buttons that were pressed during the frame the the event was called
        /// </summary>
        public MouseButtons[] ButtonsPressed
        {
            get { return _buttonsPressed; }
        }

        /// <summary>
        /// Gets an array of buttons that were released during the frame the the event was called
        /// </summary>
        public MouseButtons[] ButtonsReleased
        {
            get { return _buttonsReleased; }
        }

        /// <summary>
        /// Gets the current position mouse in screen coordinates
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
        }

        /// <summary>
        /// Gets the current X location of the mouse in screen coordinates
        /// </summary>
        public int X
        {
            get { return (int)_position.X; }
        }
        
        /// <summary>
        /// Gets the current Y location of the mouse in screen coordinates
        /// </summary>
        public int Y
        {
            get { return (int)_position.Y; }
        } 
        
        /// <summary>
        /// Gets a boolean value indicating that the event was or was not handled.
        /// </summary>
        public bool Handled
        {
            get { return _handled; }
            set { _handled = value; }
        }

        /// <summary>
        /// Gets the movement delta of the mouse from the previous to the current frame
        /// </summary>
        public Vector2 MovementDelta
        {
            get { return _movementDelta; }
        } 

        /// <summary>
        /// Creates a new instance of MouseEventArgs
        /// </summary>
        public MouseEventArgs(MouseButtons[] buttonsDown, 
            MouseButtons[] buttonsUp, MouseButtons[] buttonsPressed, MouseButtons[] buttonsReleased, Vector2 position, Vector2 movementDelta)
        {
            _position = position;
            _buttonsDown = buttonsDown;
            _buttonsUp = buttonsUp;
            _buttonsPressed = buttonsPressed;
            _buttonsReleased = buttonsReleased;
            _movementDelta = movementDelta;
        }

        /// <summary>
        /// Returns a Boolean value indicating if the button was up during the event
        /// </summary>
        public bool ContainsButtonUp(MouseButtons button)
        {
            bool found = false;

            for (int i = 0; i < _buttonsUp.Length && !found; i++)
            {
                found = _buttonsUp[i] == button;
            }

            return found;
        }

        /// <summary>
        /// Returns a Boolean value indicating if the button was down during the event
        /// </summary>
        public bool ContainsButtonDown(MouseButtons button)
        {
            bool found = false;

            for (int i = 0; i < _buttonsDown.Length && !found; i++)
            {
                found = _buttonsDown[i] == button;
            }

            return found;
        }

        /// <summary>
        /// Returns a Boolean value indicating if the button was pressed during the event
        /// </summary>
        public bool ContainsButtonPressed(MouseButtons button)
        {
            bool found = false;

            for (int i = 0; i < _buttonsPressed.Length && !found; i++)
            {
                found = _buttonsPressed[i] == button;
            }

            return found;
        }

        /// <summary>
        /// Returns a Boolean value indicating if the button was released during the event
        /// </summary>
        public bool ContainsButtonReleased(MouseButtons button)
        {
            bool found = false;

            for (int i = 0; i < _buttonsReleased.Length && !found; i++)
            {
                found = _buttonsReleased[i] == button;
            }

            return found;
        }
    }

    public class KeyboardEventArgs : EventArgs
    {
        readonly Keys[] _keysUp;
        readonly Keys[] _keysDown;
        readonly Keys[] _keysPressed;
        readonly Keys[] _keysReleased;

        bool _handled = false;

        public Keys[] KeysUp
        {
            get { return _keysUp; }
        }

        public Keys[] KeysDown
        {
            get { return _keysDown; }
        }

        public Keys[] KeysPressed
        {
            get { return _keysPressed; }
        } 

        public Keys[] KeysReleased
        {
            get { return _keysReleased; }
        } 

        public bool Handled
        {
            get { return _handled; }
            set { _handled = value; }
        }

        public KeyboardEventArgs(Keys[] keysUp, Keys[] keysDown, Keys[] keysPressed, Keys[] keysReleased)
        {
            _keysUp = keysUp;
            _keysDown = keysDown;
            _keysPressed = keysPressed;
            _keysReleased = keysReleased;
        }

        public bool ContainsKeyUp(Keys key)
        {
            bool found = false;

            for (int i = 0; i < _keysUp.Length && !found; i++)
            {
                found = _keysUp[i] == key;
            }

            return found;
        }

        public bool ContainsKeyDown(Keys key)
        {
            bool found = false;

            for (int i = 0; i < _keysDown.Length && !found; i++)
            {
                found = _keysDown[i] == key;
            }

            return found;
        }

        public bool ContainsKeyPressed(Keys key)
        {
            bool found = false;

            for (int i = 0; i < _keysPressed.Length && !found; i++)
            {
                found = _keysPressed[i] == key;
            }

            return found;
        }

        public bool ContainsKeyReleased(Keys key)
        {
            bool found = false;

            for (int i = 0; i < _keysReleased.Length && !found; i++)
            {
                found = _keysReleased[i] == key;
            }

            return found;
        }
    }
}
