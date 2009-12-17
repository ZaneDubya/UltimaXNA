using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UILegacy
{
    public class iControl
    {
        Serial Serial { get; set; }
        Vector2 Position { get; set; }
        Vector2 Size { get; set; }
        bool Enabled { get; set; }
        bool Visible { get; set; }
        bool IsDisposed { get; set;  }
    }
}
