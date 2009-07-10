using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network
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
