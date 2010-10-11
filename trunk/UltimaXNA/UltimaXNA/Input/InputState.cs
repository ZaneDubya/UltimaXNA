/********************************************************
 * 
 *  $Id: InputState.cs 80 2010-01-12 05:38:23Z jeff $
 *  
 *  $Author: jeff $
 *  $Date: 2010-01-11 21:38:23 -0800 (Mon, 11 Jan 2010) $
 *  $Revision: 80 $
 *  
 *  $LastChangedBy: jeff $
 *  $LastChangedDate: 2010-01-11 21:38:23 -0800 (Mon, 11 Jan 2010) $
 *  $LastChangedRevision: 80 $
 *  
 *  (C) Copyright 2009 Jeff Boulanger
 *  All rights reserved. 
 *  
 ********************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UltimaXNA.Input;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Input;

namespace UltimaXNA.Input
{
    /// <summary>
    /// Provides an asyncronous Input Event system that can be used to monitor Keyboard and Mouse events.
    /// </summary>
    public sealed class InputState : MessageHook, IInputService, IGameComponent, IUpdateable
    {
        public override int HookType
        {
            get { return NativeConstants.WH_GETMESSAGE; }
        }

        private bool _enabled;
        private int _updateOrder;

        /// <summary>
        /// Gets the currently pressed Modifier keys, Control, Alt, Shift
        /// </summary>
        public static Keys ModifierKeys
        {
            get
            {
                Keys none = Keys.None;

                if (NativeMethods.GetKeyState(0x10) < 0)
                {
                    none |= Keys.Shift;
                }

                if (NativeMethods.GetKeyState(0x11) < 0)
                {
                    none |= Keys.Control;
                }

                if (NativeMethods.GetKeyState(0x12) < 0)
                {
                    none |= Keys.Alt;
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

        /// <summary>
        /// 
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnEnabledChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int UpdateOrder
        {
            get { return _updateOrder; }
            set { _updateOrder = value; }
        }

        /// <summary>
        /// Raised when the user lets go of a key that is down.
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyUp;

        /// <summary>
        /// Raised when the user presses a key down
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyDown;

        /// <summary>
        /// Raised when a key is pressed down, then released
        /// </summary>
        public event EventHandler<KeyPressEventArgs> KeyPress;

        /// <summary>
        /// Raised when the user uses the Mouse Wheel
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseWheel;

        /// <summary>
        /// Raised when the mouse moves
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseMove;

        /// <summary>
        /// Raised when a Mouse Button is pressed down.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseDown;

        /// <summary>
        /// Raised when a Mouse Button is released
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseUp;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler EnabledChanged;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler UpdateOrderChanged;

        /// <summary>
        /// Creates a new instance of InputHook
        /// </summary>
        public InputState(Game game)
            : base(game.Window.Handle)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MouseState GetMouseState()
        {
            return Mouse.GetState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public KeyboardState GetKeyState()
        {
            return Keyboard.GetState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        protected override int WndProcHook(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return base.WndProcHook(nCode, wParam, lParam);
            }

            if ((int)wParam != 0)
            {
                Message message = (Message)Marshal.PtrToStructure(lParam, typeof(Message));
                NativeMethods.TranslateMessage(ref message);
                WndPrc(ref message);
            }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private void WndPrc(ref Message message)
        {
            try
            {
                Debug.WriteLine(string.Format("Message Id: {0}", message.Id));

                switch (message.Id)
                {
                    case NativeConstants.WM_KEYDOWN:
                    case NativeConstants.WM_KEYUP:
                        {
                            WmKeyEvent(ref message);
                            break;
                        }
                    case NativeConstants.WM_MOUSEMOVE:
                        {
                            WmMouseMove(ref message);
                            break;
                        }
                    case NativeConstants.WM_LBUTTONDOWN:
                        {
                            WmMouseDown(ref message, MouseButton.Left, 1);
                            break;
                        }
                    case NativeConstants.WM_RBUTTONDOWN:
                        {
                            WmMouseDown(ref message, MouseButton.Right, 1);
                            break;
                        }
                    case NativeConstants.WM_MBUTTONDOWN:
                        {
                            WmMouseDown(ref message, MouseButton.Middle, 1);
                            break;
                        }
                    case NativeConstants.WM_LBUTTONUP:
                        {
                            WmMouseUp(ref message, MouseButton.Left, 1);
                            break;
                        }
                    case NativeConstants.WM_RBUTTONUP:
                        {
                            WmMouseUp(ref message, MouseButton.Right, 1);
                            break;
                        }
                    case NativeConstants.WM_MBUTTONUP:
                        {
                            WmMouseUp(ref message, MouseButton.Middle, 1);
                            break;
                        }
                    case NativeConstants.WM_LBUTTONDBLCLK:
                        {
                            WmMouseDown(ref message, MouseButton.Left, 2);
                            break;
                        }
                    case NativeConstants.WM_RBUTTONDBLCLK:
                        {
                            WmMouseDown(ref message, MouseButton.Right, 2);
                            break;
                        }
                    case NativeConstants.WM_MBUTTONDBLCLK:
                        {
                            WmMouseDown(ref message, MouseButton.Middle, 2);
                            break;
                        }
                    case NativeConstants.WM_MOUSEWHEEL:
                        {
                            WmMouseWheel(ref message);
                            break;
                        }
                    case NativeConstants.WM_XBUTTONDOWN:
                        {
                            WmMouseDown(ref message, GetXButton(Message.HighWord(message.WParam)), 1);
                            break;
                        }
                    case NativeConstants.WM_XBUTTONUP:
                        {
                            WmMouseUp(ref message, GetXButton(Message.HighWord(message.WParam)), 1);
                            break;
                        }
                    case NativeConstants.WM_XBUTTONDBLCLK:
                        {
                            WmMouseDown(ref message, GetXButton(Message.HighWord(message.WParam)), 2);
                            break;
                        }
                }
            }
            catch
            {
                //TODO: log...crash...what?
            }
        }

        /// <summary>
        /// Gets the Mouse XButton deciphered from the wparam argument of a Message
        /// </summary>
        /// <param name="wparam"></param>
        /// <returns></returns>
        private MouseButton GetXButton(int wparam)
        {
            switch (wparam)
            {
                case 1: return MouseButton.XButton1;
                case 2: return MouseButton.XButton2;
            }

            return MouseButton.None;
        }

        /// <summary>
        /// Reads the supplied message and executes any Mouse Wheel events required.
        /// </summary>
        /// <param name="message">The Message to parse</param>
        private void WmMouseWheel(ref Message message)
        {
            OnMouseWheel(new MouseEventArgs(MouseButtons, Message.SignedHighWord(message.WParam), Message.SignedLowWord(message.LParam), Message.SignedHighWord(message.LParam), 0));
        }

        /// <summary>
        /// Reads the supplied message and executes any Mouse Move events required.
        /// </summary>
        /// <param name="message">The Message to parse</param>
        private void WmMouseMove(ref Message message)
        {
            OnMouseMove(new MouseEventArgs(MouseButtons, 0, Message.SignedLowWord(message.LParam), Message.SignedHighWord(message.LParam), 0));
        }

        /// <summary>
        /// Reads the supplied message and executes any Mouse Down events required.
        /// </summary>
        /// <param name="message">The Message to parse</param>
        /// <param name="button">The Mouse Button the Message is for</param>
        /// <param name="clicks">The number of clicks for the Message</param>
        private void WmMouseDown(ref Message message, MouseButton button, int clicks)
        {
            // HandleMouseBindings();
            OnMouseDown(new MouseEventArgs(button, clicks, Message.SignedLowWord(message.LParam), Message.SignedHighWord(message.LParam), 0));
        }

        /// <summary>
        /// Reads the supplied message and executes any Mouse Up events required.
        /// </summary>
        /// <param name="message">The Message to parse</param>
        /// <param name="button">The Mouse Button the Message is for</param>
        /// <param name="clicks">The number of clicks for the Message</param>
        private void WmMouseUp(ref Message message, MouseButton button, int clicks)
        {
            // HandleMouseBindings();
            OnMouseUp(new MouseEventArgs(button, clicks, Message.SignedLowWord(message.LParam), Message.SignedHighWord(message.LParam), 0));
        }

        /// <summary>
        /// Reads the supplied message and executes any Keyboard events required.
        /// </summary>
        /// <param name="message">The Message to parse</param>
        /// <returns>A Boolean value indicating wether the Key events were handled or not</returns>
        private bool WmKeyEvent(ref Message message)
        {
            // HandleKeyBindings();

            KeyEventArgs keyEventArgs = null;
            KeyPressEventArgs keyPressEventArgs = null;

            IntPtr zero = IntPtr.Zero;

            if ((message.Id == 0x102) || (message.Id == 0x106))
            {
                keyPressEventArgs = new KeyPressEventArgs((char)((ushort)((long)message.WParam)));
                zero = (IntPtr)keyPressEventArgs.KeyChar;
                OnKeyPress(keyPressEventArgs);
            }
            else
            {
                keyEventArgs = new KeyEventArgs(((Keys)((int)((long)message.WParam))) | ModifierKeys);

                if ((message.Id == 0x100) || (message.Id == 0x104))
                {
                    OnKeyDown(keyEventArgs);
                }
                else
                {
                    OnKeyUp(keyEventArgs);
                }
            }

            if (keyPressEventArgs != null)
            {
                message.WParam = zero;
                return keyPressEventArgs.Handled;
            }

            return keyEventArgs.Handled;
        }

        /// <summary>
        /// Raises the MouseWheel event. Override this method to add code to handle when a mouse wheel is turned
        /// </summary>
        /// <param name="e">MouseEventArgs for the MouseWheel event</param>
        private void OnMouseWheel(MouseEventArgs e)
        {
            if (MouseWheel != null)
            {
                MouseWheel(this, e);
            }
        }

        /// <summary>
        /// Raises the MouseMove event. Override this method to add code to handle when the mouse is moved
        /// </summary>
        /// <param name="e">MouseEventArgs for the MouseMove event</param>
        private void OnMouseMove(MouseEventArgs e)
        {
            if (MouseMove != null)
            {
                MouseMove(this, e);
            }
        }

        /// <summary>
        /// Raises the MouseDown event. Override this method to add code to handle when a mouse button is pressed
        /// </summary>
        /// <param name="e">MouseEventArgs for the MouseDown event</param>
        private void OnMouseDown(MouseEventArgs e)
        {
            if (MouseDown != null)
            {
                MouseDown(this, e);
            }
        }

        /// <summary>
        /// Raises the MouseUp event. Override this method to add code to handle when a mouse button is released
        /// </summary>
        /// <param name="e">MouseEventArgs for the MouseUp event</param>
        private void OnMouseUp(MouseEventArgs e)
        {
            if (MouseUp != null)
            {
                MouseUp(this, e);
            }
        }

        /// <summary>
        /// Raises the KeyUp event. Override this method to add code to handle when a key is released
        /// </summary>
        /// <param name="e">KeyboardPressEventArgs for the KeyUp event</param>
        private void OnKeyUp(KeyEventArgs e)
        {
            if (KeyUp != null)
            {
                KeyUp(this, e);
            }
        }

        /// <summary>
        /// Raises the KeyDown event. Override this method to add code to handle when a key is pressed
        /// </summary>
        /// <param name="e">KeyEventArgs for the KeyDown event</param>
        private void OnKeyDown(KeyEventArgs e)
        {
            if (KeyDown != null)
            {
                KeyDown(this, e);
            }
        }

        /// <summary>
        /// Raises the KeyPress event. Override this method to add code to handle when a key is pressed
        /// </summary>
        /// <param name="e">KeyboardPressEventArgs for the KeyPress event</param>
        private void OnKeyPress(KeyPressEventArgs e)
        {
            if (KeyPress != null)
            {
                KeyPress(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEnabledChanged(object sender, EventArgs e)
        {
            if (EnabledChanged != null)
            {
                EnabledChanged(sender, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUpdateOrderChanged(object sender, EventArgs e)
        {
            if (UpdateOrderChanged != null)
            {
                UpdateOrderChanged(sender, e);
            }
        }
    }
}
