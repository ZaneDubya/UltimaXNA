using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace UltimaXNA.Core.Extensions
{
    static class MouseStateExtensions
    {
        public static MouseState CreateWithDPI(this MouseState value, Vector2 dpi)
        {
            int x = (int)(value.X / dpi.X);
            int y = (int)(value.Y / dpi.Y);
            MouseState state = new MouseState(x, y, value.ScrollWheelValue, 
                value.LeftButton, value.MiddleButton, value.RightButton, 
                value.XButton1, value.XButton2);
            return state;
        }
    }
}
