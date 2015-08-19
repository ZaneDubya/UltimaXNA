/***************************************************************************
 *   NativeMethods.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using UltimaXNA.Core.Input;
#endregion

namespace UltimaXNA.Core
{
    class NativeMethods
    {
        [DllImport("Kernel32")]
        private unsafe static extern int _lread(SafeFileHandle hFile, void* lpBuffer, int wBytes);

        public static unsafe void Read(SafeFileHandle ptr, void* buffer, int length)
        {
            _lread(ptr, buffer, length);
        }
    }
}
