using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.Core.Resources
{
    public interface IAnimationFrame
    {
        Point Center { get; }
        Texture2D Texture { get; }
    }
}
