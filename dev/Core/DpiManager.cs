/***************************************************************************
 *   DpiManager.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace UltimaXNA.Core
{
    static class DpiManager
    {
        private const int LogPixelsX = 88; // Used for GetDeviceCaps().
        private const int LogPixelsY = 90; // Used for GetDeviceCaps().
        private const float StandardDpi = 96f; // Used for GetDeviceCaps().

        public static Vector2 GetSystemDpiScalar()
        {
            Vector2 result = new Vector2();
            IntPtr hdc = GetDC(IntPtr.Zero);

            result.X = GetDeviceCaps(hdc, LogPixelsX) / StandardDpi;
            result.Y = GetDeviceCaps(hdc, LogPixelsY) / StandardDpi;

            ReleaseDC(IntPtr.Zero, hdc);

            return result;
        }

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    }
}