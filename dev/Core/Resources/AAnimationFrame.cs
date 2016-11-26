using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.Core.Resources
{
    public abstract class AAnimationFrame
    {
        public Point Center
        {
            get;
            protected set;
        }

        public Texture2D Texture
        {
            get;
            protected set;
        }

        public abstract bool IsPointInTexture(int x, int y);
    }
}
