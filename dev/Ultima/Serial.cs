/***************************************************************************
 *   Serial.cs
 *   Based on code from RunUO: http://www.runuo.com
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings

using System;
#endregion

namespace UltimaXNA.Ultima
{
    public struct Serial : IComparable, IComparable<Serial>
    {
        public static Serial ProtectedAction = int.MinValue;

        public static Serial Null
        {
            get { return 0; }
        }

        public static Serial World
        {
            get { return unchecked((int)0xFFFFFFFF); }
        }

        private readonly int m_Serial;

        private Serial(int serial)
        {
            m_Serial = serial;
        }

        public int Value
        {
            get
            {
                return m_Serial;
            }
        }

        public bool IsMobile
        {
            get
            {
                return (m_Serial > 0 && m_Serial < 0x40000000);
            }
        }

        public bool IsItem
        {
            get
            {
                return (m_Serial >= 0x40000000);
            }
        }

        public bool IsValid
        {
            get
            {
                return (m_Serial > 0);
            }
        }

        public bool IsDynamic
        {
            get
            {
                return (m_Serial < 0);
            }
        }

        private static int s_NextDynamicSerial = -1;
        public static int NewDynamicSerial
        {
            get { return s_NextDynamicSerial--; }
        }

        public override int GetHashCode()
        {
            return m_Serial;
        }

        public int CompareTo(Serial other)
        {
            return m_Serial.CompareTo(other.m_Serial);
        }

        public int CompareTo(object other)
        {
            if (other is Serial)
                return CompareTo((Serial)other);
            else if (other == null)
                return -1;

            throw new ArgumentException();
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is Serial)) return false;

            return ((Serial)o).m_Serial == m_Serial;
        }

        public static bool operator ==(Serial l, Serial r)
        {
            return l.m_Serial == r.m_Serial;
        }

        public static bool operator !=(Serial l, Serial r)
        {
            return l.m_Serial != r.m_Serial;
        }

        public static bool operator >(Serial l, Serial r)
        {
            return l.m_Serial > r.m_Serial;
        }

        public static bool operator <(Serial l, Serial r)
        {
            return l.m_Serial < r.m_Serial;
        }

        public static bool operator >=(Serial l, Serial r)
        {
            return l.m_Serial >= r.m_Serial;
        }

        public static bool operator <=(Serial l, Serial r)
        {
            return l.m_Serial <= r.m_Serial;
        }

        public override string ToString()
        {
            return String.Format("0x{0:X8}", m_Serial);
        }

        public static implicit operator int(Serial a)
        {
            return a.m_Serial;
        }

        public static implicit operator Serial(int a)
        {
            return new Serial(a);
        }
    }
}
