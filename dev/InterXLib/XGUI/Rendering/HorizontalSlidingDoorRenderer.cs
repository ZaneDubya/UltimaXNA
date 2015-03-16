using InterXLib.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InterXLib.XGUI.Rendering
{
    class HorizontalSlidingDoorRenderer : ARenderer
    {
        private readonly Rectangle m_center, m_edge;
        private readonly int m_buffer;

        public int Buffer
        {
            get { return m_buffer; }
        }

        public HorizontalSlidingDoorRenderer(Texture2D texture, Rectangle source, int center, int edge, int buffer)
            : base(texture)
        {
            m_center = new Rectangle(source.Left, source.Top, center, source.Height);
            m_edge = new Rectangle(source.Left + center, source.Top, edge, source.Height);
            m_buffer = buffer;
        }

        public override Rectangle BuildChildArea(Point area)
        {
            return new Rectangle(0, 0, area.X, area.Y);
        }

        public override void Render(YSpriteBatch batch, Rectangle destination, Color? color = null)
        {
            Color Color = (color == null) ? Color.White : color.Value;
            Rectangle drawArea = new Rectangle(destination.Left, destination.Top, m_edge.Width, m_edge.Height);

            batch.GUIDrawSprite(Texture, drawArea, m_edge, Color, SpriteEffects.FlipHorizontally);

            drawArea.X = destination.Right - m_edge.Width;
            batch.GUIDrawSprite(Texture, drawArea, m_edge, Color);

            drawArea.X = destination.Left + m_edge.Width;
            drawArea.Width = destination.Width - (2 * m_edge.Width);
            batch.GUIDrawSprite(Texture, drawArea, m_center, Color);
        }
    }
}
