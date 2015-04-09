/***************************************************************************
 *   SharedMethods.cs
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
#endregion

namespace UltimaXNA.UltimaData
{
    class SharedMethods
    {
        public static bool InRange(IPoint2D from, IPoint2D to, int range)
        {
            return (from.X >= (to.X - range)) && (from.X <= (to.X + range)) && (from.Y >= (to.Y - range)) && (from.Y <= (to.Y + range));
        }

        [DllImport("Kernel32")]
        private unsafe static extern int m_lread(SafeFileHandle hFile, void* lpBuffer, int wBytes);

        public static unsafe void Read(SafeFileHandle ptr, void* buffer, int length)
        {
            m_lread(ptr, buffer, length);
        }
    }
}
