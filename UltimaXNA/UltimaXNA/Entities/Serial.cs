/***************************************************************************
 *   Serial.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace UltimaXNA
{
    public struct Serial : IComparable, IComparable<Serial>
    {
        private int _serial;

        private Serial(int serial)
        {
            _serial = serial;
        }

        public int Value
        {
            get
            {
                return _serial;
            }
        }

        public bool IsMobile
        {
            get
            {
                return (_serial > 0 && _serial < 0x40000000);
            }
        }

        public bool IsItem
        {
            get
            {
                return (_serial >= 0x40000000 && _serial <= 0x7FFFFFFF);
            }
        }

        public bool IsValid
        {
            get
            {
                return (_serial > 0);
            }
        }

        public override int GetHashCode()
        {
            return _serial;
        }

        public int CompareTo(Serial other)
        {
            return _serial.CompareTo(other._serial);
        }

        public int CompareTo(object other)
        {
            if (other is Serial)
                return this.CompareTo((Serial)other);
            else if (other == null)
                return -1;

            throw new ArgumentException();
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is Serial)) return false;

            return ((Serial)o)._serial == _serial;
        }

        public static bool operator ==(Serial l, Serial r)
        {
            return l._serial == r._serial;
        }

        public static bool operator !=(Serial l, Serial r)
        {
            return l._serial != r._serial;
        }

        public static bool operator >(Serial l, Serial r)
        {
            return l._serial > r._serial;
        }

        public static bool operator <(Serial l, Serial r)
        {
            return l._serial < r._serial;
        }

        public static bool operator >=(Serial l, Serial r)
        {
            return l._serial >= r._serial;
        }

        public static bool operator <=(Serial l, Serial r)
        {
            return l._serial <= r._serial;
        }

        public override string ToString()
        {
            return String.Format("0x{0:X8}", _serial);
        }

        public static implicit operator int(Serial a)
        {
            return a._serial;
        }

        public static implicit operator Serial(int a)
        {
            return new Serial(a);
        }
    }
}
