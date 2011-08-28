/***************************************************************************
 *   IControl.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UILegacy
{
    public class IControl
    {
        Serial Serial { get; set; }
        Vector2 Position { get; set; }
        Vector2 Size { get; set; }
        bool Enabled { get; set; }
        bool Visible { get; set; }
        bool IsDisposed { get; set;  }
    }
}
