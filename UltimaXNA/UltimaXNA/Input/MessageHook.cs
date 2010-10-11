using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;

namespace UltimaXNA.Input
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="nCode"></param>
    /// <param name="wParam"></param>
    /// <param name="lParam"></param>
    /// <returns></returns>
    public delegate int WndProcHandler(int nCode, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// 
    /// </summary>
    public abstract class MessageHook : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public abstract int HookType { get; }

        private IntPtr hHook;
        private IntPtr hWnd;
        private WndProcHandler cachedHook;

        /// <summary>
        /// 
        /// </summary>
        public IntPtr HHook
        {
            get { return hHook; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IntPtr HWnd
        {
            get { return hWnd; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        public MessageHook(IntPtr hWnd)
        {
            this.hWnd = hWnd;

            cachedHook = WndProcHook;
            CreateHook();
        }

        /// <summary>
        /// 
        /// </summary>
        ~MessageHook()
        {
            Dispose(false);
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateHook()
        {
            uint threadId = NativeMethods.GetWindowThreadProcessId(hWnd, IntPtr.Zero);
            hHook = NativeMethods.SetWindowsHookEx(HookType, cachedHook, IntPtr.Zero, threadId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        protected virtual int WndProcHook(int nCode, IntPtr wParam, IntPtr lParam)
        {
            return NativeMethods.CallNextHookEx(hHook, nCode, wParam, lParam);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

            }

            if (hHook != IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(hHook);
            }
        }
    }
}
