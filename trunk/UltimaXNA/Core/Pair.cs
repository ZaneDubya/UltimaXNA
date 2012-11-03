/***************************************************************************
 *   Pair.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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

namespace UltimaXNA
{
    public struct Pair<T, U>
    {
        public T ItemA;
        public U ItemB;

        public Pair(T a, U b)
        {
            ItemA = a;
            ItemB = b;
        }
    }
}
