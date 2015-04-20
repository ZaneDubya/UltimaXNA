using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Core.Graphics
{
    // N.B. Techniques must be numbered sequentially! Any missing numbers might cause the shader to crash.
    public enum Techniques : int
    {
        Hued = 0,
        MiniMap = 1,

        Default = Hued,
        Max = MiniMap
    }
}
