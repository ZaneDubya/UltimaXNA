using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace UltimaXNA.Ultima.UI.Interfaces
{
    interface IScrollBar
    {
        int Value { get; set; }
        int MinValue { get; set; }
        int MaxValue { get; set; }
        Point Position { get; set; }
        int Width { get; set; }
        int Height { get; set; }

        bool PointWithinControl(int x, int y);
    }
}
