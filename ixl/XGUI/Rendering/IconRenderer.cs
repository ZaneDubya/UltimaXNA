using InterXLib.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InterXLib.XGUI.Rendering
{
    class IconRenderer : ARenderer
    {
        private Rectangle m_Icon;

        public IconRenderer(Texture2D texture, Rectangle source)
            : base(texture)
        {
            m_Icon = source;
        }

        public override Rectangle BuildChildArea(Point area)
        {
            return new Rectangle(0, 0, area.X, area.Y);
        }

        public override void Render(YSpriteBatch batch, Rectangle area, Color? color = null)
        {
            Color Color = (color == null) ? Color.White : color.Value;
            // ##### Draw Icon ##### //
            Rectangle drawArea = new Rectangle(
                area.Left,
                area.Top,
                m_Icon.Width,
                m_Icon.Height);
            batch.GUIDrawSprite(Texture, drawArea, m_Icon, Color);
        }
    }
}
