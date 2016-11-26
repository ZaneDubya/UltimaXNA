using Microsoft.Xna.Framework;

namespace UltimaXNA.Core.Extensions
{
    public static class PointExtensions
    {
        public static Point DivideBy(this Point value, int divisor)
        {
            value.X /= divisor;
            value.Y /= divisor;
            return value;
        }
    }
}
