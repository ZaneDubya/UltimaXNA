/********************************************************
 * 
 *  MessageHook.cs
 *  
 *  (C) Copyright 2009 Jeff Boulanger. All rights reserved. 
 *  Used in UltimaXNA with permission.
 *  
 ********************************************************/

using System;
using System.Runtime.InteropServices;

namespace InterXLib.Input.Windows
{
    public delegate IntPtr WndProcHandler(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    public abstract class MessageHook : IDisposable
    {
        public abstract int HookType { get; }

        private IntPtr m_hWnd;
        private WndProcHandler m_Hook;
        private IntPtr m_prevWndProc;
        private IntPtr m_hIMC;

        public IntPtr HWnd
        {
            get { return m_hWnd; }
        }

        public MessageHook(IntPtr hWnd)
        {
            m_hWnd = hWnd;
            m_Hook = WndProcHook;
            m_prevWndProc = (IntPtr)NativeMethods.SetWindowLong(
                hWnd,
                NativeConstants.GWL_WNDPROC, (int)Marshal.GetFunctionPointerForDelegate(m_Hook));
            m_hIMC = NativeMethods.ImmGetContext(m_hWnd);
            new InputMessageFilter(m_Hook);
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
                        NativeMethods.ImmAssociateContext(hWnd, m_hIMC);
                    break;
                case NativeConstants.WM_INPUTLANGCHANGE:
                    int rrr = (int)NativeMethods.CallWindowProc(m_prevWndProc, hWnd, msg, wParam, lParam);
                    NativeMethods.ImmAssociateContext(hWnd, m_hIMC);
                    
                    return (IntPtr)1;
            }

            return NativeMethods.CallWindowProc(m_prevWndProc, hWnd, msg, wParam, lParam);
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
        private WndProcHandler m_Hook;

        public InputMessageFilter(WndProcHandler hook)
        {
            m_Hook = hook;
            System.Windows.Forms.Application.AddMessageFilter(this);
        }
        [DllImport("user32.dll", EntryPoint = "TranslateMessage")]
        protected extern static bool m_TranslateMessage(ref System.Windows.Forms.Message m);

        bool System.Windows.Forms.IMessageFilter.PreFilterMessage(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case NativeConstants.WM_SYSKEYDOWN:
                case NativeConstants.WM_SYSKEYUP:
                    {
                        bool b = m_TranslateMessage(ref m);
                        m_Hook(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
                        return true;
                    }

                case NativeConstants.WM_SYSCHAR:
                    {
                        m_Hook(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
                        return true;
                    }
                case NativeConstants.WM_KEYDOWN:
                case NativeConstants.WM_KEYUP:
                    {
                        bool b = m_TranslateMessage(ref m);
                        m_Hook(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
                        return true;
                    }
                case NativeConstants.WM_CHAR:
                    {
                        m_Hook(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
                        return true;
                    }
                case NativeConstants.WM_DEADCHAR:
                    {
                        m_Hook(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
                        return true;
                    }
            }
            return false;
        }
    }
}
