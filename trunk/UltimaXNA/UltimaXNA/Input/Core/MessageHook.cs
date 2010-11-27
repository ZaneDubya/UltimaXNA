/********************************************************
 * 
 *  MessageHook.cs
 *  
 *  (C) Copyright 2009 Jeff Boulanger. All rights reserved. 
 *  Used in UltimaXNA with permission.
 *  
 ********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;

namespace UltimaXNA.Input.Core
{
    public delegate IntPtr WndProcHandler(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    public abstract class MessageHook : IDisposable
    {
        public abstract int HookType { get; }

        private IntPtr _hWnd;
        private WndProcHandler _Hook;
        private IntPtr _prevWndProc;
        private IntPtr _hIMC;

        public IntPtr HWnd
        {
            get { return _hWnd; }
        }

        public MessageHook(IntPtr hWnd)
        {
            _hWnd = hWnd;
            _Hook = WndProcHook;
            _prevWndProc = (IntPtr)NativeMethods.SetWindowLong(
                hWnd,
                NativeConstants.GWL_WNDPROC, (int)Marshal.GetFunctionPointerForDelegate(_Hook));
            _hIMC = NativeMethods.ImmGetContext(_hWnd);
            new InputMessageFilter(_Hook);
        }

        ~MessageHook()
        {
            Dispose(false);
        }

        protected virtual IntPtr WndProcHook(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case NativeConstants.WM_GETDLGCODE:
                    return (IntPtr)(NativeConstants.DLGC_WANTALLKEYS);
               case NativeConstants.WM_IME_SETCONTEXT:
                    if ((int)wParam == 1)
                        NativeMethods.ImmAssociateContext(hWnd, _hIMC);
                    break;
                case NativeConstants.WM_INPUTLANGCHANGE:
                    int rrr = (int)NativeMethods.CallWindowProc(_prevWndProc, hWnd, msg, wParam, lParam);
                    NativeMethods.ImmAssociateContext(hWnd, _hIMC);
                    
                    return (IntPtr)1;
            }

            return NativeMethods.CallWindowProc(_prevWndProc, hWnd, msg, wParam, lParam);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {

        }
    }

    // This is the class that brings back the alt messages
    // http://www.gamedev.net/community/forums/topic.asp?topic_id=554322
    class InputMessageFilter : System.Windows.Forms.IMessageFilter
    {
        private WndProcHandler _Hook;

        public InputMessageFilter(WndProcHandler hook)
        {
            _Hook = hook;
            System.Windows.Forms.Application.AddMessageFilter(this);
        }
        [DllImport("user32.dll", EntryPoint = "TranslateMessage")]
        protected extern static bool _TranslateMessage(ref System.Windows.Forms.Message m);

        bool System.Windows.Forms.IMessageFilter.PreFilterMessage(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case NativeConstants.WM_SYSKEYDOWN:
                case NativeConstants.WM_SYSKEYUP:
                    {
                        bool b = _TranslateMessage(ref m);
                        _Hook(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
                        return true;
                    }

                case NativeConstants.WM_SYSCHAR:
                    {
                        _Hook(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
                        return true;
                    }
                case NativeConstants.WM_KEYDOWN:
                case NativeConstants.WM_KEYUP:
                    {
                        bool b = _TranslateMessage(ref m);
                        _Hook(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
                        return true;
                    }
                case NativeConstants.WM_CHAR:
                    {
                        _Hook(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
                        return true;
                    }
                case NativeConstants.WM_DEADCHAR:
                    {
                        _Hook(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
                        return true;
                    }
            }
            return false;
        }
    }
}
