using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace UltimaXNA.Input.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Message
    {
        public UInt32 Id;
        public IntPtr WParam;
        public IntPtr LParam;
        public Point2D Point;

        public Message(uint id, IntPtr wParam, IntPtr lParam)
        {
            this.Id = id;
            this.WParam = wParam;
            this.LParam = lParam;
            this.Point = new Point2D(LowWord(lParam), HighWord(lParam));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Message))
            {
                return false;
            }

            Message msg = (Message)obj;

            return msg.Id == Id && msg.LParam == LParam && msg.Point == Point && msg.WParam == WParam;
        }

        public override int GetHashCode()
        {
            return (int)Id;
        }

        public static bool operator ==(Message m1, Message m2)
        {
            return m1.Equals(m2);
        }

        public static bool operator !=(Message m1, Message m2)
        {
            return !m1.Equals(m2);
        }

        public static int HighWord(int n)
        {
            return ((n >> 0x10) & 0xffff);
        }

        public static int HighWord(IntPtr n)
        {
            return HighWord((int)((long)n));
        }

        public static int LowWord(int n)
        {
            return (n & 0xffff);
        }

        public static int LowWord(IntPtr n)
        {
            return LowWord((int)((long)n));
        }

        public static int MakeLong(int low, int high)
        {
            return ((high << 0x10) | (low & 0xffff));
        }

        public static IntPtr MakeLParam(int low, int high)
        {
            return (IntPtr)((high << 0x10) | (low & 0xffff));
        }

        public static int SignedHighWord(int n)
        {
            return (short)((n >> 0x10) & 0xffff);
        }

        public static int SignedHighWord(IntPtr n)
        {
            return SignedHighWord((int)((long)n));
        }

        public static int SignedLowWord(int n)
        {
            return (short)(n & 0xffff);
        }

        public static int SignedLowWord(IntPtr n)
        {
            return SignedLowWord((int)((long)n));
        }
    }
}
