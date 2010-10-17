using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Input.Unused
{
    class Bindings
    {
        private Dictionary<WinKeys, InputBinding> _keyBindings;
        private Dictionary<MouseButton, InputBinding> _mouseBindings;

        /// <summary>
        /// Gets the currently pressed Modifier keys, Control, Alt, Shift
        /// </summary>
        public static WinKeys ModifierKeys
        {
            get
            {
                WinKeys none = WinKeys.None;

                if (NativeMethods.GetKeyState(0x10) < 0)
                {
                    none |= WinKeys.Shift;
                }

                if (NativeMethods.GetKeyState(0x11) < 0)
                {
                    none |= WinKeys.Control;
                }

                if (NativeMethods.GetKeyState(0x12) < 0)
                {
                    none |= WinKeys.Alt;
                }

                return none;
            }
        }

        /// <summary>
        /// Gets the current pressed Mouse Buttons
        /// </summary>
        public static MouseButton MouseButtons
        {
            get
            {
                MouseButton none = MouseButton.None;

                if (NativeMethods.GetKeyState(1) < 0)
                {
                    none |= MouseButton.Left;
                }

                if (NativeMethods.GetKeyState(2) < 0)
                {
                    none |= MouseButton.Right;
                }

                if (NativeMethods.GetKeyState(4) < 0)
                {
                    none |= MouseButton.Middle;
                }

                if (NativeMethods.GetKeyState(5) < 0)
                {
                    none |= MouseButton.XButton1;
                }

                if (NativeMethods.GetKeyState(6) < 0)
                {
                    none |= MouseButton.XButton2;
                }

                return none;
            }
        }

        public Bindings()
        {
            _keyBindings = new Dictionary<WinKeys, InputBinding>();
            _mouseBindings = new Dictionary<MouseButton, InputBinding>();
        }

        /// <summary>
        /// Creates and adds a binding to the InputState for the supplied Keyboard Key and Modifier Keys.
        /// </summary>
        /// <param name="name">The name of the binding</param>
        /// <param name="shift">Binding requires shift button in order to execute</param>
        /// <param name="control">Binding requires control button in order to execute</param>
        /// <param name="alt">Binding requires alt button in order to execute</param>
        /// <param name="key">The Key to bind to.</param>
        /// <param name="beginHandler">The handler to execute when the binding's key combination is pressed down</param>
        /// <returns>The InputBinding object.</returns>
        public InputBinding AddBinding(string name, bool shift, bool control, bool alt, WinKeys key, EventHandler handler)
        {
            return AddBinding(name, shift, control, alt, key, handler, null);
        }

        /// <summary>
        /// Creates and adds a binding to the InputState for the supplied Keyboard Key and Modifier Keys.
        /// </summary>
        /// <param name="name">The name of the binding</param>
        /// <param name="shift">Binding requires shift button in order to execute</param>
        /// <param name="control">Binding requires control button in order to execute</param>
        /// <param name="alt">Binding requires alt button in order to execute</param>
        /// <param name="key">The Key to bind to.</param>
        /// <param name="beginHandler">The handler to execute when the binding's key combination is pressed down</param>
        /// <param name="endHandler">The handler to execute when the binding's key combination button is pressed release</param>
        /// <returns>The InputBinding object.</returns>
        public InputBinding AddBinding(string name, bool shift, bool control, bool alt, WinKeys key, EventHandler beginHandler, EventHandler endHandler)
        {
            InputBinding binding = new InputBinding(name, shift, control, alt);

            binding.BeginExecution = beginHandler;
            binding.EndExecution = endHandler;

            key |= shift ? WinKeys.Shift : WinKeys.None;
            key |= control ? WinKeys.Control : WinKeys.None;
            key |= alt ? WinKeys.Alt : WinKeys.None;

            _keyBindings.Add(key, binding);

            return binding;
        }

        /// <summary>
        /// Creates a binding for the supplied Mouse Button.  The callback will call the handler on MouseDown.
        /// </summary>
        /// <param name="name">The name of the binding</param>
        /// <param name="buttons">The Mouse Button to attach the binding to</param>
        /// <param name="beginHandler">The handler to execute when the binding's mouse button is pressed down</param>
        /// <returns>The InputBinding object.</returns>
        public InputBinding AddBinding(string name, MouseButton buttons, EventHandler handler)
        {
            return AddBinding(name, buttons, handler, null);
        }

        /// <summary>
        /// Creates a binding for the supplied Mouse Button.  The callback will call beginHandler on MouseDown and endHandler on MouseUp
        /// </summary>
        /// <param name="name">The name of the binding</param>
        /// <param name="buttons">The Mouse Button to attach the binding to</param>
        /// <param name="beginHandler">The handler to execute when the binding's mouse button is pressed down</param>
        /// <param name="endHandler">The handler to execute when the binding's mouse button is pressed release</param>
        /// <returns>The InputBinding object.</returns>
        public InputBinding AddBinding(string name, MouseButton buttons, EventHandler beginHandler, EventHandler endHandler)
        {
            InputBinding binding = new InputBinding(name, false, false, false);

            binding.BeginExecution = beginHandler;
            binding.EndExecution = endHandler;

            _mouseBindings.Add(buttons, binding);

            return binding;
        }

        /// <summary>
        /// Handles and manages any InputBindings hooked to Key Input
        /// </summary>
        private void HandleKeyBindings()
        {
            foreach (WinKeys keys in _keyBindings.Keys)
            {
                WinKeys key = keys;
                InputBinding binding = _keyBindings[keys];

                WinKeys modifiers = binding.ModifierKeys;

                //Remove any modifiers so we can 
                //get the exact key...
                key = keys & ~WinKeys.Shift;
                key = keys & ~WinKeys.Alt;
                key = keys & ~WinKeys.Control;

                binding.IsExecuting = ((NativeMethods.GetKeyState((int)key) < 0) &&
                                       ((ModifierKeys & modifiers) == modifiers));
            }
        }

        /// <summary>
        /// Handles and manages any InputBindings hooked to Mouse Input
        /// </summary>
        private void HandleMouseBindings()
        {
            foreach (MouseButton button in _mouseBindings.Keys)
            {
                _mouseBindings[button].IsExecuting = ((MouseButtons & button) == button);
            }
        }
    }
}
