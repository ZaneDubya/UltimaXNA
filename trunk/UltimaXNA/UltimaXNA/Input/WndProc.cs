﻿/********************************************************
 * 
 *  WndProc.cs
 *  
 *  (C) Copyright 2009 Jeff Boulanger. All rights reserved. 
 *  Used in UltimaXNA with permission.
 *  
 ********************************************************/

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
    /// <summary>
    /// Provides an asyncronous Input Event system that can be used to monitor Keyboard and Mouse events.
    /// </summary>
    public class WndProc : MessageHook
    {
        public override int HookType
        {
            get { return NativeConstants.WH_GETMESSAGE; }
        }

        protected WndProc(IntPtr hWnd)
            : base(hWnd)
        {
            
        }

        protected MouseState getMouseState()
        {
            return Mouse.GetState();
        }

        protected KeyboardState getKeyState()
        {
            return Keyboard.GetState();
        }

        /// <summary>
        /// Gets the currently pressed Modifier keys, Control, Alt, Shift
        /// </summary>
        protected WinKeys getModifierKeys()
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

        /// <summary>
        /// Gets the current pressed Mouse Buttons
        /// </summary>
        protected MouseButton getMouseButtons(MouseState state)
        {
            MouseButton none = MouseButton.None;

            if (state.LeftButton == ButtonState.Pressed)
                none |= MouseButton.Left;
            if (state.RightButton == ButtonState.Pressed)
                none |= MouseButton.Right;
            if (state.MiddleButton == ButtonState.Pressed)
                none |= MouseButton.Middle;
            if (state.XButton1 == ButtonState.Pressed)
                none |= MouseButton.XButton1;
            if (state.XButton2 == ButtonState.Pressed)
                none |= MouseButton.XButton2;

            return none;
        }
        
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
            OnMouseWheel(new MouseEventArgs(getMouseButtons(getMouseState()), Message.SignedHighWord(message.WParam), Message.SignedLowWord(message.LParam), Message.SignedHighWord(message.LParam), 0));
        }

        /// <summary>
        /// Reads the supplied message and executes any Mouse Move events required.
        /// </summary>
        /// <param name="message">The Message to parse</param>
        private void WmMouseMove(ref Message message)
        {
            OnMouseMove(new MouseEventArgs(getMouseButtons(getMouseState()), 0, Message.SignedLowWord(message.LParam), Message.SignedHighWord(message.LParam), 0));
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
                keyEventArgs = new KeyEventArgs(
                    (WinKeys)(((int)(long)message.WParam) | ((int)getModifierKeys()))
                    );

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
        protected virtual void OnMouseWheel(MouseEventArgs e)
        {

        }

        /// <summary>
        /// Raises the MouseMove event. Override this method to add code to handle when the mouse is moved
        /// </summary>
        /// <param name="e">MouseEventArgs for the MouseMove event</param>
        protected virtual void OnMouseMove(MouseEventArgs e)
        {

        }

        /// <summary>
        /// Raises the MouseDown event. Override this method to add code to handle when a mouse button is pressed
        /// </summary>
        /// <param name="e">MouseEventArgs for the MouseDown event</param>
        protected virtual void OnMouseDown(MouseEventArgs e)
        {

        }

        /// <summary>
        /// Raises the MouseUp event. Override this method to add code to handle when a mouse button is released
        /// </summary>
        /// <param name="e">MouseEventArgs for the MouseUp event</param>
        protected virtual void OnMouseUp(MouseEventArgs e)
        {

        }

        /// <summary>
        /// Raises the KeyUp event. Override this method to add code to handle when a key is released
        /// </summary>
        /// <param name="e">KeyboardPressEventArgs for the KeyUp event</param>
        protected virtual void OnKeyUp(KeyEventArgs e)
        {

        }

        /// <summary>
        /// Raises the KeyDown event. Override this method to add code to handle when a key is pressed
        /// </summary>
        /// <param name="e">KeyEventArgs for the KeyDown event</param>
        protected virtual void OnKeyDown(KeyEventArgs e)
        {

        }

        /// <summary>
        /// Raises the KeyPress event. Override this method to add code to handle when a key is pressed
        /// </summary>
        /// <param name="e">KeyboardPressEventArgs for the KeyPress event</param>
        protected virtual void OnKeyPress(KeyPressEventArgs e)
        {

        }
    }
}
