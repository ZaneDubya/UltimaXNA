/***************************************************************************
 *   Techniques.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

namespace UltimaXNA.Core.Graphics
{
    // N.B. Techniques must be numbered sequentially! Any missing numbers might cause the shader to crash.
    public enum Techniques : int
    {
        // drawn effects:
        Hued = 0,
        MiniMap = 1,
        Grayscale = 2,
        // shadow effects
        ShadowSet = 3,
        ShadowClear = 4,
        // stencil effects ( not currently used )
        StencilSet = 5, 
        StencilClear = 6,

        Default = Hued,
        FirstDrawn = Hued,
        LastDrawn = Grayscale,
        FirstShadow = ShadowSet,
        LastShadow = ShadowClear,
        FirstStencil = StencilSet,
        LastStencil = StencilClear,
        All = StencilClear
    }
}
