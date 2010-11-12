using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Input
{
    public enum MouseButtonInternal
    {
        Left = 0x100000,
        Middle = 0x400000,
        None = 0,
        Right = 0x200000,
        XButton1 = 0x800000,
        XButton2 = 0x1000000
    }

    public enum MouseButton
    {
        Left = 0,
        Right = 1,
        Middle = 2,
        XButton1 = 3,
        XButton2 = 4,
        None = 0x7f
    }
}
