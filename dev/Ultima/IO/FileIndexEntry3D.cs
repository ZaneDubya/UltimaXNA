/***************************************************************************
 *   FileIndexEntry5D.cs
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Runtime.InteropServices;
#endregion

namespace UltimaXNA.Ultima.IO
{
    [StructLayout(LayoutKind.Sequential, Pack = 0x1)]
    public struct FileIndexEntry3D
    {
        public int Lookup;
        public int Length;
        public int Extra;

        public FileIndexEntry3D(int lookup, int length, int extra)
        {
            Lookup = lookup;
            Length = length;
            Extra = extra;
        }
    }
}
