using Microsoft.Xna.Framework;

namespace UltimaXNA.Core.UI
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

        bool IsVisible { get; set; } // from AControl
    }
}
