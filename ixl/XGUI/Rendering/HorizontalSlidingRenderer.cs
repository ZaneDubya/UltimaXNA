using InterXLib.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InterXLib.XGUI.Rendering
{
    class HorizontalSlidingRenderer : ARenderer
    {
        private Rectangle m_Center, m_Left, m_Right;
        private bool m_LeftBorderIsRightBorderReversed = true;
        public bool DrawLeftBorderAsRightBorder
        {
            get { return m_LeftBorderIsRightBorderReversed; }
            set { m_LeftBorderIsRightBorderReversed = value; }
        }

        public HorizontalSlidingRenderer(Texture2D texture, Rectangle source, int left_border, int right_border)
            : base(texture)
        {
            m_Left = new Rectangle(source.Left, source.Top, left_border, source.Height);
            m_Right = new Rectangle(source.Right - right_border, source.Top, right_border, source.Height);
            m_Center = new Rectangle(source.Left + left_border, source.Top, source.Width - left_border - right_border, source.Height);
        }

        public override Rectangle BuildChildArea(Point area)
        {
            return new Rectangle(0, 0, area.X, area.Y);
        }

        public override void Render(YSpriteBatch batch, Rectangle area, Color? color = null)
        {
            Color Color = (color == null) ? Color.White : color.Value;
            Rectangle drawArea;

            int left_width = m_LeftBorderIsRightBorderReversed ? m_Right.Width : m_Left.Width;

            // ##### Draw Left portion ##### //
            drawArea = new Rectangle(
                area.Left,
                area.Top,
                left_width,
                m_Left.Height);
            batch.GUIDrawSprite(Texture, drawArea, 
                m_LeftBorderIsRightBorderReversed ? m_Right : m_Left, Color, 
                effects: m_LeftBorderIsRightBorderReversed ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

            // ##### Draw Center portion ##### //
            drawArea = new Rectangle(
                area.Left + left_width,
                area.Top,
                area.Width - left_width - m_Right.Width,
                m_Center.Height);
            batch.GUIDrawSprite(Texture, drawArea, m_Center, Color);

            // ##### Draw Right portion ##### //
            drawArea = new Rectangle(
                area.Right - m_Right.Width,
                area.Top,
                m_Right.Width,
                m_Right.Height);
            batch.GUIDrawSprite(Texture, drawArea, m_Right, Color);
        }
    }
}
