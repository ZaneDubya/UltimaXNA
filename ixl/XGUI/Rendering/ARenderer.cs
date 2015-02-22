using InterXLib.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InterXLib.XGUI.Rendering
{
    public abstract class ARenderer
    {
        protected Texture2D Texture { get; set; }

        abstract public Rectangle BuildChildArea(Point size);
        abstract public void Render(YSpriteBatch spritebatch, Rectangle area, Color? color = null);

        protected ARenderer(Texture2D texture)
        {
            Texture = texture;
        }
    }
}
