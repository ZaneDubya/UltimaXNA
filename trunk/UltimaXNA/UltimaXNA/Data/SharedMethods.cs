#region File Description & Usings
//-----------------------------------------------------------------------------
// SharedMethods.cs
//
// Based on UltimaSDK, modifications by Poplicola
//-----------------------------------------------------------------------------
using System;
using System.Runtime.InteropServices;
#endregion

namespace UltimaXNA.Data
{
    class NativeMethods
    {
        [DllImport("Kernel32")]
        private unsafe static extern int _lread(IntPtr hFile, void* lpBuffer, int wBytes);

        public static unsafe void Read(IntPtr ptr, void* buffer, int length)
        {
            _lread(ptr, buffer, length);
        }
    }

    interface IPoint2D
    {
        int X { get; }
        int Y { get; }
    }

    class Helpers
    {
        public static bool InRange(IPoint2D from, IPoint2D to, int range)
        {
            return (from.X >= (to.X - range)) && (from.X <= (to.X + range)) && (from.Y >= (to.Y - range)) && (from.Y <= (to.Y + range));
        }
    }

    class Point2D : IPoint2D
    {
        #region X
        private int m_X;
        public int X
        {
            get { return m_X; }
            set { m_X = value; }
        }
        #endregion

        #region Y
        private int m_Y;
        public int Y
        {
            get { return m_Y; }
            set { m_Y = value; }
        }
        #endregion

        public Point2D(int x, int y)
        {
            m_X = x;
            m_Y = y;
        }
    }
}
