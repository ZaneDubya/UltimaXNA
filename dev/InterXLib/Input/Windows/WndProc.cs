/********************************************************
 * 
 *  WndProc.cs
 *  
 *  (C) Copyright 2009 Jeff Boulanger. All rights reserved. 
 *  Used in UltimaXNA with permission.
 *  
 ********************************************************/

using System;
using Microsoft.Xna.Framework.Input;

namespace InterXLib.Input.Windows
{
    public delegate void MouseEventHandler(InputEventMouse e);
    public delegate void KeyboardEventHandler(InputEventKeyboard e);

    /// <summary>
    /// Provides an asyncronous Input Event system that can be used to monitor Keyboard and Mouse events.
    /// </summary>
    public class WndProc : MessageHook
    {
        const bool WP_PASSTHROUGH = true;
        const bool WP_NOPASSTHROUGH = false;

        public override int HookType
        {
            get { return NativeConstants.WH_CALLWNDPROC; }
        }

        public WndProc(IntPtr hWnd)
            : base(hWnd)
        {
            
        }

        public MouseState MouseState
        {
            get
            {
                return Mouse.GetState();
            }
        }

        /// <summary>
        /// Gets the currently pressed Modifier keys, Control, Alt, Shift
        /// </summary>
        public WinKeys ModifierKeys
        {
            get
            {
                WinKeys keys = WinKeys.None;

                if (NativeMethods.GetKeyState((int)WinKeys.ShiftKey) < 0)
                {
                    keys |= WinKeys.Shift;
                }

                if (NativeMethods.GetKeyState((int)WinKeys.ControlKey) < 0)
                {
                    keys |= WinKeys.Control;
                }

                if (NativeMethods.GetKeyState((int)WinKeys.Menu) < 0)
                {
                    keys |= WinKeys.Alt;
                }

                return keys;
            }
        }

        /// <summary>
        /// Gets the current pressed Mouse Buttons
        /// </summary>
        public MouseButtonInternal MouseButtons(MouseState state)
        {
            MouseButtonInternal none = MouseButtonInternal.None;

            if (state.LeftButton == ButtonState.Pressed)
                none |= MouseButtonInternal.Left;
            if (state.RightButton == ButtonState.Pressed)
                none |= MouseButtonInternal.Right;
            if (state.MiddleButton == ButtonState.Pressed)
                none |= MouseButtonInternal.Middle;
            if (state.XButton1 == ButtonState.Pressed)
                none |= MouseButtonInternal.XButton1;
            if (state.XButton2 == ButtonState.Pressed)
                none |= MouseButtonInternal.XButton2;

            return none;
        }

        protected override IntPtr WndProcHook(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            Message message = new Message(msg, wParam, lParam);
            if (WndPrc(ref message) == WP_NOPASSTHROUGH)
                return IntPtr.Zero;
            return base.WndProcHook(hWnd, msg, wParam, lParam);
        }

        protected bool WndPrc(ref Message message)
        {
            try
            {
                switch (message.Id)
                {
                    case NativeConstants.WM_DEADCHAR:
                        {
                            break;
                        }
                    case NativeConstants.WM_KEYDOWN:
                    case NativeConstants.WM_KEYUP:
                    case NativeConstants.WM_CHAR:
                        {
                            
                            WmKeyEvent(ref message);
                            
                            break;
                        }
                    case NativeConstants.WM_SYSKEYDOWN:
                    case NativeConstants.WM_SYSKEYUP:
                    case NativeConstants.WM_SYSCHAR:
                        {
                            NativeMethods.TranslateMessage(ref message);
                            WmKeyEvent(ref message);
                            return WP_NOPASSTHROUGH;
                        }
                    case NativeConstants.WM_SYSCOMMAND:
                        {
                            break;
                        }
                    case NativeConstants.WM_MOUSEMOVE:
                        {
                            WmMouseMove(ref message);
                            break;
                        }
                    case NativeConstants.WM_LBUTTONDOWN:
                        {
                            WmMouseDown(ref message, MouseButtonInternal.Left, 1);
                            break;
                        }
                    case NativeConstants.WM_RBUTTONDOWN:
                        {
                            WmMouseDown(ref message, MouseButtonInternal.Right, 1);
                            break;
                        }
                    case NativeConstants.WM_MBUTTONDOWN:
                        {
                            WmMouseDown(ref message, MouseButtonInternal.Middle, 1);
                            break;
                        }
                    case NativeConstants.WM_LBUTTONUP:
                        {
                            WmMouseUp(ref message, MouseButtonInternal.Left, 1);
                            return WP_PASSTHROUGH;
                        }
                    case NativeConstants.WM_RBUTTONUP:
                        {
                            WmMouseUp(ref message, MouseButtonInternal.Right, 1);
                            return WP_PASSTHROUGH;
                        }
                    case NativeConstants.WM_MBUTTONUP:
                        {
                            WmMouseUp(ref message, MouseButtonInternal.Middle, 1);
                            return WP_PASSTHROUGH;
                        }
                    case NativeConstants.WM_LBUTTONDBLCLK:
                        {
                            WmMouseDown(ref message, MouseButtonInternal.Left, 2);
                            return WP_PASSTHROUGH;
                        }
                    case NativeConstants.WM_RBUTTONDBLCLK:
                        {
                            WmMouseDown(ref message, MouseButtonInternal.Right, 2);
                            return WP_PASSTHROUGH;
                        }
                    case NativeConstants.WM_MBUTTONDBLCLK:
                        {
                            WmMouseDown(ref message, MouseButtonInternal.Middle, 2);
                            return WP_PASSTHROUGH;
                        }
                    case NativeConstants.WM_MOUSEWHEEL:
                        {
                            WmMouseWheel(ref message);
                            return WP_PASSTHROUGH;
                        }
                    case NativeConstants.WM_XBUTTONDOWN:
                        {
                            WmMouseDown(ref message, GetXButton(Message.HighWord(message.WParam)), 1);
                            return WP_PASSTHROUGH;
                        }
                    case NativeConstants.WM_XBUTTONUP:
                        {
                            WmMouseUp(ref message, GetXButton(Message.HighWord(message.WParam)), 1);
                            return WP_PASSTHROUGH;
                        }
                    case NativeConstants.WM_XBUTTONDBLCLK:
                        {
                            WmMouseDown(ref message, GetXButton(Message.HighWord(message.WParam)), 2);
                            return WP_PASSTHROUGH;
                        }
                }
            }
            catch
            {
                //TODO: log...crash...what?
            }

            return WP_PASSTHROUGH;
        }

        private MouseButtonInternal translateWParamIntoMouseButtons(int wParam)
        {
            MouseButtonInternal mb = MouseButtonInternal.None;
            if ((wParam & 0x0001) == 0x0001)
                mb |= MouseButtonInternal.Left;
            if ((wParam & 0x0002) == 0x0002)
                mb |= MouseButtonInternal.Right;
            if ((wParam & 0x0002) == 0x0010)
                mb |= MouseButtonInternal.Middle;
            if ((wParam & 0x0002) == 0x0020)
                mb |= MouseButtonInternal.XButton1;
            if ((wParam & 0x0002) == 0x0040)
                mb |= MouseButtonInternal.XButton2;
            return mb;
        }

        /// <summary>
        /// Gets the Mouse XButton deciphered from the wparam argument of a Message
        /// </summary>
        /// <param name="wparam"></param>
        /// <returns></returns>
        private MouseButtonInternal GetXButton(int wparam)
        {
            switch (wparam)
            {
                case 1: return MouseButtonInternal.XButton1;
                case 2: return MouseButtonInternal.XButton2;
            }

            return MouseButtonInternal.None;
        }

        /// <summary>
        /// Reads the supplied message and executes any Mouse Wheel events required.
        /// </summary>
        /// <param name="message">The Message to parse</param>
        private void WmMouseWheel(ref Message message)
        {
            invokeMouseWheel(new InputEventMouse(MouseEvent.WheelScroll,
                translateWParamIntoMouseButtons(Message.SignedLowWord(message.WParam)),
                Message.SignedHighWord(message.WParam), 
                Message.SignedLowWord(message.LParam), 
                Message.SignedHighWord(message.LParam),
                (int)(long)message.WParam,
                ModifierKeys
                ));
        }

        /// <summary>
        /// Reads the supplied message and executes any Mouse Move events required.
        /// </summary>
        /// <param name="message">The Message to parse</param>
        private void WmMouseMove(ref Message message)
        {
            invokeMouseMove(new InputEventMouse(MouseEvent.Move,
                translateWParamIntoMouseButtons(Message.SignedLowWord(message.WParam)),
                0, 
                message.Point.X, 
                message.Point.Y,
                (int)(long)message.WParam,
                ModifierKeys
                ));
        }

        /// <summary>
        /// Reads the supplied message and executes any Mouse Down events required.
        /// </summary>
        /// <param name="message">The Message to parse</param>
        /// <param name="button">The Mouse Button the Message is for</param>
        /// <param name="clicks">The number of clicks for the Message</param>
        private void WmMouseDown(ref Message message, MouseButtonInternal button, int clicks)
        {
            // HandleMouseBindings();
            invokeMouseDown(new InputEventMouse(MouseEvent.Down,
                button, 
                clicks, 
                Message.SignedLowWord(message.LParam), 
                Message.SignedHighWord(message.LParam),
                (int)(long)message.WParam,
                ModifierKeys
                ));
        }

        /// <summary>
        /// Reads the supplied message and executes any Mouse Up events required.
        /// </summary>
        /// <param name="message">The Message to parse</param>
        /// <param name="button">The Mouse Button the Message is for</param>
        /// <param name="clicks">The number of clicks for the Message</param>
        private void WmMouseUp(ref Message message, MouseButtonInternal button, int clicks)
        {
            // HandleMouseBindings();
            invokeMouseUp(new InputEventMouse(MouseEvent.Up,
                button, 
                clicks, 
                Message.SignedLowWord(message.LParam), 
                Message.SignedHighWord(message.LParam),
                (int)(long)message.WParam,
                ModifierKeys
                ));
        }

        /// <summary>
        /// Reads the supplied message and executes any Keyboard events required.
        /// </summary>
        /// <param name="message">The Message to parse</param>
        /// <returns>A Boolean value indicating wether the Key events were handled or not</returns>
        private void WmKeyEvent(ref Message message)
        {
            // HandleKeyBindings();
            // KeyPressEventArgs keyPressEventArgs = null;

            if ((message.Id == NativeConstants.WM_CHAR) || (message.Id == NativeConstants.WM_SYSCHAR))
            {
                // Is this extra information necessary?
                // wm_(sys)char: http://msdn.microsoft.com/en-us/library/ms646276(VS.85).aspx

                InputEventKeyboard e = new InputEventKeyboard(KeyboardEventType.Press,
                    (WinKeys)(int)(long)message.WParam,
                    (int)(long)message.LParam,
                    ModifierKeys
                    );
                IntPtr zero = (IntPtr)0;// (char)((ushort)((long)message.WParam));
                invokeChar(e);
            }
            else
            {
                // wm_(sys)keydown: http://msdn.microsoft.com/en-us/library/ms912654.aspx
                // wm_(sys)keyup: http://msdn.microsoft.com/en-us/library/ms646281(VS.85).aspx


                if ((message.Id == NativeConstants.WM_KEYDOWN) || (message.Id == NativeConstants.WM_SYSKEYDOWN))
                {
                    InputEventKeyboard e = new InputEventKeyboard(KeyboardEventType.Down,
                        (WinKeys)(int)(long)message.WParam,
                        (int)(long)message.LParam,
                        ModifierKeys
                        );
                    invokeKeyDown(e);
                }
                else if ((message.Id == NativeConstants.WM_KEYUP) || (message.Id == NativeConstants.WM_SYSKEYUP))
                {
                    InputEventKeyboard e = new InputEventKeyboard(KeyboardEventType.Up,
                        (WinKeys)(int)(long)message.WParam,
                        (int)(long)message.LParam,
                        ModifierKeys
                        );
                    invokeKeyUp(e);
                }
            }
        }

        public event MouseEventHandler MouseWheel, MouseMove, MouseDown, MouseUp;
        public event KeyboardEventHandler KeyUp, KeyDown, KeyChar;

        /// <summary>
        /// Raises the MouseWheel event. Override this method to add code to handle when a mouse wheel is turned
        /// </summary>
        /// <param name="e">InputEventCM for the MouseWheel event</param>
        private void invokeMouseWheel(InputEventMouse e)
        {
            if (MouseWheel != null)
                MouseWheel(e);
        }

        /// <summary>
        /// Raises the MouseMove event. Override this method to add code to handle when the mouse is moved
        /// </summary>
        /// <param name="e">InputEventCM for the MouseMove event</param>
        private void invokeMouseMove(InputEventMouse e)
        {
            if (MouseMove != null)
                MouseMove(e);
        }

        /// <summary>
        /// Raises the MouseDown event. Override this method to add code to handle when a mouse button is pressed
        /// </summary>
        /// <param name="e">InputEventCM for the MouseDown event</param>
        private void invokeMouseDown(InputEventMouse e)
        {
            if (MouseDown != null)
                MouseDown(e);
        }

        /// <summary>
        /// Raises the MouseUp event. Override this method to add code to handle when a mouse button is released
        /// </summary>
        /// <param name="e">InputEventCM for the MouseUp event</param>
        private void invokeMouseUp(InputEventMouse e)
        {
            if (MouseUp != null)
                MouseUp(e);
        }

        /// <summary>
        /// Raises the KeyUp event. Override this method to add code to handle when a key is released
        /// </summary>
        /// <param name="e">KeyboardPressEventArgs for the KeyUp event</param>
        private void invokeKeyUp(InputEventKeyboard e)
        {
            if (KeyUp != null)
                KeyUp(e);
        }

        /// <summary>
        /// Raises the KeyDown event. Override this method to add code to handle when a key is pressed
        /// </summary>
        /// <param name="e">InputEventCKB for the KeyDown event</param>
        private void invokeKeyDown(InputEventKeyboard e)
        {
            if (KeyDown != null)
                KeyDown(e);
        }
        
        /// <summary>
        /// Raises the OnChar event. Override this method to add code to handle when a WM_CHAR message is received
        /// </summary>
        /// <param name="e">InputEventCKB for the OnChar event</param>
        private void invokeChar(InputEventKeyboard e)
        {
            if (KeyChar != null)
                KeyChar(e);
        }
    }
}
